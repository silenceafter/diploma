using auth.Data;
using auth.Services.Interfaces;
using auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using auth.DTOs;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Security.Claims;
using Newtonsoft.Json;
using auth.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using auth;

var builder = WebApplication.CreateBuilder(args);

//appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//bind
var settingsJwtDto = new SettingsJwtDto();
builder.Configuration.GetSection("JWT").Bind(settingsJwtDto);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 37))));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;

    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<CustomIdentityErrorDescriber>();

//cors
builder.Services.AddCors(options =>
    options.AddPolicy("policy", builder =>
    {
        builder.WithOrigins("https://localhost:5173", "https://localhost:5174", "https://localhost:7158")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    })
);

//appsettings: jwt
builder.Services.Configure<SettingsJwtDto>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//локализация
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ru")
    };

    options.DefaultRequestCulture = new RequestCulture("ru");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

//контроллеры, сервисы, ..
builder.Services.AddControllers();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddTransient<EmailService>();

//аутентификация
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = settingsJwtDto.Issuer,
            ValidAudience = settingsJwtDto.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settingsJwtDto.SecretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                Console.WriteLine("OnTokenValidated called");
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var token = context.SecurityToken as JwtSecurityToken;
                var tokenId = token?.RawData;
                
                //клеймы
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                var nameClaim = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
                var emailClaim = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
                var roleClaim = claimsIdentity?.FindFirst(ClaimTypes.Role)?.Value;
                //
                if (!string.IsNullOrEmpty(nameClaim))
                    claimsIdentity?.AddClaim(new Claim(ClaimTypes.Name, nameClaim));
                if (!string.IsNullOrEmpty(emailClaim))
                    claimsIdentity?.AddClaim(new Claim(ClaimTypes.Email, emailClaim));
                if (!string.IsNullOrEmpty(roleClaim))
                    claimsIdentity?.AddClaim(new Claim(ClaimTypes.Role, roleClaim));

                //черный список токенов
                if (dbContext.BlacklistedTokens.Any(t => t.Token == tokenId))
                {
                    context.Fail("This token is blacklisted.");
                }
                await Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new { message = "Authentication failed" });
                return context.Response.WriteAsync(result);
            }
        };
    });

//авторизация
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowIfNoRoleClaim", policy =>
        policy.RequireAssertion(context =>
        {
            //если токен не содержит claim с ролью, разрешить доступ
            return !context.User.Claims.Any(c => c.Type == "roles");
        }));
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });

    // Add JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API V1");
        c.RoutePrefix = string.Empty; // This makes swagger the default page
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors("policy");

//локацизация
var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
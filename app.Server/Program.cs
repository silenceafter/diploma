using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;
using app.Server.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using app.Server.Repositories.Interfaces;
using app.Server.Repositories;
using app.Server.Services.Interfaces;
using app.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using app.Server;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;
using auth.Models;
using auth.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//bind
var settingsJwt = new SettingsJwt();
builder.Configuration.GetSection("JWT").Bind(settingsJwt);

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
builder.Services.Configure<SettingsJwt>(builder.Configuration.GetSection("JWT"));

// Add services to the container.
builder.Services.AddControllers()
    /*.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    })*/;

//identity
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        /*options.RequireHttpsMetadata = false;
        options.SaveToken = true;*/
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = settingsJwt.Issuer,
            ValidAudience = settingsJwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settingsJwt.SecretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var tokenValidationService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidationService>();
                var token = context.SecurityToken as JwtSecurityToken;
                var tokenId = token?.RawData;

                if (!await tokenValidationService.Validate(tokenId))
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowIfNoRoleClaim", policy =>
        policy.RequireAssertion(context =>
        {
            //если токен не содержит claim с ролью, разрешить доступ
            return !context.User.Claims.Any(c => c.Type == "roles");
        }));
});

builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITokenValidationService, TokenValidationService>();
builder.Services.AddTransient<ICAuthorizationService, CAuthorizationService>();
builder.Services.AddTransient<IAcceptanceRepository, AcceptanceRepository>();
builder.Services.AddTransient<IHazardousWasteRepository, HazardousWasteRepository>();
builder.Services.AddTransient<IPartnerRepository, PartnerRepository>();
builder.Services.AddTransient<IHazardClassRepository, HazardClassRepository>();
builder.Services.AddTransient<IReceivingDiscountRepository, ReceivingDiscountRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IPointRepository, PointRepository>();
builder.Services.AddDbContext<EcodbContext>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend API", Version = "v1" });

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("policy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");
app.Run();

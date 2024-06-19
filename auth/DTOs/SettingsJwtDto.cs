namespace auth.DTOs
{
    public class SettingsJwtDto
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string ExpirationMinutes { get; set; }
    }
}

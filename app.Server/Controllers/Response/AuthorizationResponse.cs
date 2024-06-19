namespace app.Server.Controllers.Response
{
    public class AuthorizationResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
    }
}

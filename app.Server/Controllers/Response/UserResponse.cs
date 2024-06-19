namespace app.Server.Controllers.Response
{
    public class UserResponse
    {
        public string Id { get; set; }//удалить
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; }
        public int Bonus { get; set; }
        public string Role { get; set; }
    }
}

namespace graphqlServer.Controllers.Auth
{
    public class UserDTO
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string[] Policies { get; set; } = Array.Empty<string>();        
    }
}
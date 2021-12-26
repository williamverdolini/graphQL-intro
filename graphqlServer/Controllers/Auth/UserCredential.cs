using System.ComponentModel.DataAnnotations;

namespace graphqlServer.Controllers.Auth
{
    public class UserCredential
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
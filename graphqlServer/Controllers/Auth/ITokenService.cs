namespace graphqlServer.Controllers.Auth
{

    public interface ITokenService
    {
        string BuildToken(string key, string issuer, UserDTO user);
        bool IsTokenValid(string key, string issuer, string token);
    }
}
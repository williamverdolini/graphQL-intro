namespace graphqlServer.Controllers.Auth
{
    public interface IUserRepository
    {
        UserDTO? GetUserByCredential(UserCredential credential);
        UserDTO? GetUserByName(string name);
        string[] GetUserNames();
    }
}
namespace graphqlServer.Controllers.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly List<UserDTO> users = new List<UserDTO>();

        public UserRepository()
        {
            users.Add(new UserDTO
            {
                UserName = "william.verdolini",
                Password = "xyzxyz321321",
                Role = "admin",
                Policies = new [] {"publishers.read"}
            });
            users.Add(new UserDTO
            {
                UserName = "gianmaria.giorgetti",
                Password = "xyzxyz321321",
                Role = "admin"
            });
            users.Add(new UserDTO
            {
                UserName = "alessandro.balducci",
                Password = "xyzxyz321321",
                Role = "user"
            });
            users.Add(new UserDTO
            {
                UserName = "andrea.ricci",
                Password = "xyzxyz321321",
                Role = "user",
                Policies = new [] {"publishers.read"}
            });
        }
        public UserDTO? GetUserByCredential(UserCredential credential)
        {
            return users
                .Where(x => x.UserName != null
                    && credential?.UserName != null
                    && x.UserName.ToLower() == credential.UserName.ToLower()
                    && x.Password == credential!.Password)
                .FirstOrDefault();
        }

        public UserDTO? GetUserByName(string name)
        {
            return users
                .Where(x => x.UserName != null
                    && name != null
                    && x.UserName.ToLower() == name.ToLower())
                .FirstOrDefault();
        }
    }
}
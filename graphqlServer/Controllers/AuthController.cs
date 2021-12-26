using graphqlServer.Controllers.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace graphqlServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private string generatedToken = string.Empty;
        public AuthController(IConfiguration config, ITokenService tokenService, IUserRepository userRepository)
        {
            _config = config;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public string Login(UserCredential userModel)
        {
            if (string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
            {
                return "Error";
            }

            IActionResult response = Unauthorized();
            var validUser = GetUser(userModel);

            if (validUser != null)
            {
                generatedToken = _tokenService.BuildToken(
                    _config["Jwt:Key"].ToString(), 
                    _config["Jwt:Issuer"].ToString(),
                    validUser);

                if (generatedToken != null)
                {
                    return generatedToken;
                }
                else
                {
                    return "Error";
                }
            }
            else
            {
                return "Error";
            }
        }
        private UserDTO? GetUser(UserCredential userModel)
        {
            return _userRepository.GetUserByCredential(userModel);
        }

        [Authorize]
        [Route("check-auth")]
        [HttpGet]
        public string CheckAuth()
        {
            string token = HttpContext.Request.Headers["Authorization"]; 
            // string token = HttpContext.Session.GetString("Token");

            if (token == null)
            {
                return "NOT AUTENTICATED!!!";
            }

            if (!_tokenService.IsTokenValid(
                _config["Jwt:Key"].ToString(),
                _config["Jwt:Issuer"].ToString(), 
                token))
            {
                return "NOT AUTENTICATED!!!";
            }
            return "YEEEEEEEEEEEEEES!!!";
        }
    }
}
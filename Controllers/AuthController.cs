using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Application.Interfaces.Services;
using Habit_Battles.Models.UserModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Habit_Battles.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;


        public AuthController(IUserService userService, IIdentityService identityService, IConfiguration config, IUserRepository userRepository)
        {
            _userService = userService;
            _identityService = identityService;
            _config = config;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest model)
        {
            var user = await _userService.Login(model);

            if (user.IsSuccessful)
            {
                var token = _identityService.GenerateToken(_config["Jwt:Key"], _config["Jwt:Issuer"], user.Value);
                return Ok(new { token, user.Value, user.Message });
            }
            else
            {
                return StatusCode(400, user.Message);
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromForm] string tokenId)
        {
            var user = await _userService.GoogleLogin(tokenId);

            if (user.IsSuccessful == true && user.Message == "User logged in successfully")
            {
                var token = _identityService.GenerateToken(_config["Jwt:Key"], _config["Jwt:Issuer"], user.Value);
                return Ok(new { token, user.Value, user.Message });
            }
            if (user.IsSuccessful == true && user.Message == "Google user created successfully")
            {
                return Ok(new { user.Value, user.Message });
            }
            else
            {
                return StatusCode(400, user.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRequestModel request)
        {
            try
            {
                var user = await _userService.CreateUser(request);

                if (user == null)
                    return StatusCode(500, "User service returned null");

                if (!user.IsSuccessful)
                    return StatusCode(400, user.Message ?? "Registration failed");

                if (user.Value == null)
                    return StatusCode(500, "User object was null after creation");

                var response = new LoginResponse
                {
                    Email = user.Value.Email,
                    UserName = user.Value.UserName,
                    Id = user.Value.Id
                };
                var token = _identityService.GenerateToken(_config["Jwt:Key"], _config["Jwt:Issuer"],response);

                return Ok(new { token, user.Value, user.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}

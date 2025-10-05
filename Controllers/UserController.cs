using Habit_Battles.Core.Application.Interfaces.Services;
using Habit_Battles.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Habit_Battles.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users.Value);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userService.GetCurrentUserAsync();
            if (!user.IsSuccessful || user == null)
            {
                _logger.LogError("User not found: {UserId}", user.Value.Id);
                return NotFound(new { Message = user.Message });
            }
            var result = new JsonResult(user.Value)
            {
                StatusCode = (int?)HttpStatusCode.OK
            };
            return result;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserRequestModel request)
        {
            var user = await _userService.GetCurrentUserAsync();
            var userResponse = await _userService.UpdateUser(user.Value.Id, request);
            if (userResponse.IsSuccessful)
            {
                _logger.LogInformation("User updated successfully: {UserId}", user.Value.Id);
                return Ok(new { Message = userResponse.Message });
            }
            _logger.LogError("User update failed: {UserMessage}", userResponse.Message);
            return BadRequest(new { Message = userResponse.Message });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await _userService.GetCurrentUserAsync();
            var userResponse = await _userService.RemoveUser(user.Value.Id);
            if (userResponse.IsSuccessful)
            {
                _logger.LogInformation("User deleted successfully: {UserId}", user.Value.Id);
                return Ok(new { Message = userResponse.Message });
            }
            _logger.LogError("User deletion failed: {UserMessage}", userResponse.Message);
            return BadRequest(new { Message = userResponse.Message });
        }
       
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userService.GetCurrentUserAsync();
            var response = await _userService.GetProfile(user.Value.Id);

            if (response == null || !response.IsSuccessful)
            {
                return NotFound(new { message = "User profile not found" });
            }

            return Ok(response);
        }
    }
}

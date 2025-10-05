using Habit_Battles.Core.Application.Interfaces.Services;
using Habit_Battles.Models.BattleModel;
using Habit_Battles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Habit_Battles.Core.Domain.Entities;

namespace Habit_Battles.Controllers
{
    [Route("api/battle")]
    [ApiController]
    public class BattleController : ControllerBase
    {
        private readonly IBattleService _battleService;
        private readonly IUserService _userService;

        public BattleController(IBattleService battleService, IUserService userService)
        {
            _battleService = battleService;
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBattle([FromBody] BattleRequest request)
        {
            var user = await _userService.GetCurrentUserAsync();
            if(!user.IsSuccessful)
            {
                return BadRequest("Access token is required");
            }
            var response = await _battleService.CreateBattle(request, user.Value.Id);
            if (response.IsSuccessful)
            {
                return Ok(new { user.Value, user.Message });
            }
            else
            {
                return StatusCode(400, user.Message);
            }
        }

        [HttpPost("{battleId}/accept/{opponentId}")]
        public async Task<IActionResult> AcceptBattle(int battleId, int opponentId)
        {
            var response = await _battleService.AcceptBattle(battleId, opponentId);
            if (response.IsSuccessful)
            {
                return Ok(new { response.Value, response.Message });
            }
            return StatusCode(400, response.Message);
        }

        [HttpGet("{battleId}/status")]
        public async Task<IActionResult> GetBattleStatus(int battleId)
        {
            var response = await _battleService.GetBattleStatusAsync(battleId);
            if (response.IsSuccessful)
            {
                return Ok(new { response.Value, response.Message });
            }
            return StatusCode(400, response.Message);
        }

        [HttpPost("{battleId}/end/{winnerId}")]
        public async Task<IActionResult> EndBattle(int battleId, int winnerId)
        {
            var response = await _battleService.EndBattleAsync(battleId, winnerId);
            if (response == null) return NotFound("Battle not found");
            return Ok(response);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderBoard()
        {
            var response = await _battleService.GetLeaderBoardAsync();
            return Ok(response);
        }


        [HttpPost("{battleId}/habit-strike")]
        public async Task<IActionResult> LogHabitStrike(int battleId, bool realtime, bool isSuccess)
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.Value == null)
                    return Unauthorized(new { message = "Authentication token is required" });

                var response = await _battleService.HandleStrikeAsync(battleId, user.Value.Id,realtime,isSuccess);
                if (response == null) return NotFound("Battle not found or inactive");
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBattles()
        {
            var response = await _battleService.GetActiveBattlesAsync();
            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("process-daily-result")]
        public async Task<IActionResult> ProcessDailyResult()
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user?.Value == null)
                return Unauthorized(new { message = "Authentication token is required" });

            var response = await _battleService.ProcessDailyResultsForUserAsync(user.Value.Id);
            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("process-daily-results")]
        public async Task<IActionResult> ProcessDailyResults()
        {
            var response = await _battleService.ProcessDailyResultsAsync();
            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }
    }
}

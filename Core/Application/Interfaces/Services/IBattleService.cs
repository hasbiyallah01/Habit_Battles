using Habit_Battles.Models.UserModel;
using Habit_Battles.Models;
using Habit_Battles.Models.BattleModel;

namespace Habit_Battles.Core.Application.Interfaces.Services
{
    public interface IBattleService
    {
        Task<BaseResponse<IEnumerable<BattleResponse>>> GetActiveBattlesAsync();
        Task<BaseResponse<object>> ProcessDailyResultsAsync();
        Task<BaseResponse<BattleResponse>> CreateBattle(BattleRequest request, int UserId);
        Task<BaseResponse<BattleAcceptResponse>> AcceptBattle(int battleId, int userId);
        Task<BaseResponse<BattleResponse?>> GetBattleStatusAsync(int battleId);
        Task<BaseResponse<EndBattleResponse>> EndBattleAsync(int battleId, int winnerId);
        Task<BaseResponse<IEnumerable<LeaderBoardResponse>>> GetLeaderBoardAsync();
        Task<BaseResponse<object>> HandleStrikeAsync(int battleId, int userId, bool isRealTime = false, bool isSuccess = true);
        Task<BaseResponse<object>> ProcessDailyResultsForUserAsync(int userId);
    }
}

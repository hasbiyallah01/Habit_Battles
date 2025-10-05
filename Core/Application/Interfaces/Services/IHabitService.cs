using Habit_Battles.Models;
using Habit_Battles.Models.HabitModel;

namespace Habit_Battles.Core.Application.Interfaces.Services
{
    public interface IHabitService
    {
        Task<BaseResponse<HabitResponse>> CreateHabit(HabitRequest request);
        Task<BaseResponse<HabitResponse>> UpdateHabit(int id, HabitRequest request);
        Task<bool> DeleteHabit(int id);
        Task<BaseResponse<HabitResponse>> GetHabitByUserId(int id);
        Task<BaseResponse<HabitResponse>> GetHabitById(int id);
    }
}

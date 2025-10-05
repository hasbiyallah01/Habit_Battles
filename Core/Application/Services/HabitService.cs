using Habit_Battles.Core.Application.Interfaces.Services;
using Habit_Battles.Models;
using Habit_Battles.Models.HabitModel;

namespace Habit_Battles.Core.Application.Services
{
    public class HabitService : IHabitService
    {
        public Task<BaseResponse<HabitResponse>> CreateHabit(HabitRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteHabit(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<HabitResponse>> GetHabitById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<HabitResponse>> GetHabitByUserId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<HabitResponse>> UpdateHabit(int id, HabitRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

using Habit_Battles.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Habit_Battles.Core.Application.Interfaces.Repositories
{
    public interface IHabitLogsRepository
    {
        Task<HabitLog> AddAsync(HabitLog log);
        Task<HabitLog> GetAsync(int id);
        Task<HabitLog> GetAsync(Expression<Func<HabitLog, bool>> exp);
        Task<ICollection<HabitLog>> GetAllAsync();
        void Remove(HabitLog log);
        HabitLog Update(HabitLog log);
        Task<bool> ExistsAsync(int id);
    }
}

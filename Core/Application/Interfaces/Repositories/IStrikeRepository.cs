using Habit_Battles.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Habit_Battles.Core.Application.Interfaces.Repositories
{
    public interface IStrikeRepository
    {
        Task<Strike> AddAsync(Strike strike);
        Task<Strike> GetAsync(int id);
        Task<Strike> GetAsync(Expression<Func<Strike, bool>> exp);
        Task<ICollection<Strike>> GetAllAsync();
        Task<ICollection<Strike>> GetAllAsync(int battleId);
        void Remove(Strike strike);
        Strike Update(Strike strike);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Strike>> GetByBattleAndUserAsync(int battleId, int userId);
        Task<bool> HasStrikeForTodayAsync(int battleId, int userId);
    }
}

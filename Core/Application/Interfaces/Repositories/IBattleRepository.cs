using Habit_Battles.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Habit_Battles.Core.Application.Interfaces.Repositories
{
    public interface IBattleRepository
    {
        Task<Battle> AddAsync(Battle userBattle);
        Task<Battle> GetAsync(int id);
        Task<IEnumerable<Battle>> GetActiveBattlesAsync();
        Task<IEnumerable<Battle>> GetActiveBattlesByUserAsync(int id);
        Task<Battle> GetAsync(Expression<Func<Battle, bool>> exp);
        Task<ICollection<Battle>> GetAllAsync();
        void Remove(Battle userBattle);
        Battle Update(Battle userBattle);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Battle>> GetByUserIdAsync(int userId);
    }
}

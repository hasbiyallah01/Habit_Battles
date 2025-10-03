using Habit_Battles.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Habit_Battles.Core.Application.Interfaces.Repositories
{
    public interface IUserBattleRepository
    {
        Task<UserBattle> AddAsync(UserBattle userBattle);
        Task<UserBattle> GetAsync(int id);
        Task<UserBattle> GetAsync(Expression<Func<UserBattle, bool>> exp);
        Task<ICollection<UserBattle>> GetAllAsync();
        void Remove(UserBattle userBattle);
        UserBattle Update(UserBattle userBattle);
        Task<bool> ExistsAsync(int id);
    }
}

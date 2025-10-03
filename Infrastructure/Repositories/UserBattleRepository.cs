using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Habit_Battles.Infrastructure.Repositories
{
    public class UserBattleRepository : IUserBattleRepository
    {
        public Task<UserBattle> AddAsync(UserBattle userBattle)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<UserBattle>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserBattle> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserBattle> GetAsync(Expression<Func<UserBattle, bool>> exp)
        {
            throw new NotImplementedException();
        }

        public void Remove(UserBattle userBattle)
        {
            throw new NotImplementedException();
        }

        public UserBattle Update(UserBattle userBattle)
        {
            throw new NotImplementedException();
        }
    }
}

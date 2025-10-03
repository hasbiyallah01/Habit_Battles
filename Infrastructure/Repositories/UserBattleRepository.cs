using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Habit_Battles.Infrastructure.Repositories
{
    public class UserBattleRepository : IUserBattleRepository
    {
        private readonly HabitBattlesContext _context;
        public UserBattleRepository(HabitBattlesContext context)
        {
            _context = context;
        }

        public async Task<UserBattle> AddAsync(UserBattle userBattle)
        {
            await _context.Set<UserBattle>()
                .AddAsync(userBattle);
            _context.SaveChanges();

            return userBattle;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.UserBattles.AnyAsync(a => a.Id == id);
        }

        public async Task<ICollection<UserBattle>> GetAllAsync()
        {
            return await _context.Set<UserBattle>()
                .ToListAsync();
        }

        public async Task<UserBattle> GetAsync(int id)
        {
            var answer = await _context.Set<UserBattle>()
                .Where(a => a.Id == id && !a.IsDeleted)
                .SingleOrDefaultAsync();
            return answer;
        }

        public async Task<UserBattle> GetAsync(Expression<Func<UserBattle, bool>> exp)
        {
             var answer = await _context.Set<UserBattle>()
                .FirstOrDefaultAsync(exp);
            return answer;
        }

        public void Remove(UserBattle userBattle)
        {
            userBattle.IsDeleted = true;
            _context.Set<UserBattle>()
                .Update(userBattle);

        }

        public UserBattle Update(UserBattle userBattle)
        {
            _context.Set<UserBattle>()
                .Update(userBattle);
            return userBattle;
        }
    }
}

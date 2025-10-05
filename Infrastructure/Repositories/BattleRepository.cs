using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Core.Domain.Enums;
using Habit_Battles.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Habit_Battles.Infrastructure.Repositories
{
    public class BattleRepository : IBattleRepository
    {
        private readonly HabitBattlesContext _context;
        public BattleRepository(HabitBattlesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Battle>> GetActiveBattlesAsync()
        {
            return await _context.Battles
                .Where(b => b.Status == Status.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Battle>> GetActiveBattlesByUserAsync(int id)
        {
            return await _context.Battles
                .Where(b => b.Status == Status.Active && b.Id == id)
                .ToListAsync();
        }
        public async Task<Battle> AddAsync(Battle userBattle)
        {
            await _context.Set<Battle>()
                .AddAsync(userBattle);
            _context.SaveChanges();

            return userBattle;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.UserBattles.AnyAsync(a => a.Id == id);
        }

        public async Task<ICollection<Battle>> GetAllAsync()
        {
            return await _context.Set<Battle>()
                .ToListAsync();
        }

        public async Task<Battle> GetAsync(int id)
        {
            var answer = await _context.Set<Battle>()
                .Where(a => a.Id == id && !a.IsDeleted)
                .SingleOrDefaultAsync();
            return answer;
        }

        public async Task<Battle> GetAsync(Expression<Func<Battle, bool>> exp)
        {
             var answer = await _context.Set<Battle>()
                .FirstOrDefaultAsync(exp);
            return answer;
        }

        public void Remove(Battle userBattle)
        {
            userBattle.IsDeleted = true;
            _context.Set<Battle>()
                .Update(userBattle);
            _context.SaveChangesAsync();

        }

        public Battle Update(Battle userBattle)
        {
            _context.Set<Battle>()
                .Update(userBattle);

            _context.SaveChangesAsync();
            return userBattle;
        }
    }
}

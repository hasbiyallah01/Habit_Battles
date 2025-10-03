using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Habit_Battles.Infrastructure.Repositories
{
    public class StrikeRepository : IStrikeRepository
    {
        private readonly HabitBattlesContext _context;
        private StrikeRepository(HabitBattlesContext context)
        {
            _context = context;
        }
        public async Task<Strike> AddAsync(Strike strike)
        {
            await _context.Set<Strike>()
                .AddAsync(strike);
            _context.SaveChanges();
            return strike;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Strikes.AnyAsync(a => a.Id == id);
        }

        public async Task<ICollection<Strike>> GetAllAsync()
        {
            return await _context.Set<Strike>()
                .ToListAsync();
        }

        public async Task<Strike> GetAsync(int id)
        {
            var answer = await _context.Set<Strike>()
                .Where(a => a.Id == id && !a.IsDeleted)
                .SingleOrDefaultAsync();
            return answer;
        }

        public Task<Strike> GetAsync(Expression<Func<Strike, bool>> exp)
        {
            var answer = _context.Set<Strike>()
                .Where(a => a.IsDeleted)
                .FirstOrDefaultAsync(exp);
            return answer;
        }

        public void Remove(Strike strike)
        {
            strike.IsDeleted = true;
            _context.Set<Strike>()
                .Update(strike);
            _context.SaveChangesAsync();
        }

        public Strike Update(Strike strike)
        {
            _context.Strikes.Update(strike);
            return strike;
        }
    }
}

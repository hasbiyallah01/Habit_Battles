using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Habit_Battles.Infrastructure.Repositories
{
    public class HabitLogsRepository : IHabitLogsRepository
    {
        private readonly HabitBattlesContext _context;
        public HabitLogsRepository(HabitBattlesContext context) 
        {
            _context = context;
        }

        public async Task<HabitLog> AddAsync(HabitLog log)
        {
            await _context.Set<HabitLog>()
                .AddAsync(log);
            return log;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.HabitLogs.AnyAsync(x => x.Id == id);
        }

        public async Task<ICollection<HabitLog>> GetAllAsync()
        {
            var answer = await _context.Set<HabitLog>()
                            .ToListAsync();
            return answer;
        }

        public async Task<HabitLog> GetAsync(int id)
        {
            var answer = await _context.Set<HabitLog>()
                        .Where(a => !a.IsDeleted && a.Id == id)
                        .SingleOrDefaultAsync();
            return answer;
        }

        public async Task<HabitLog> GetAsync(Expression<Func<HabitLog, bool>> exp)
        {
            var answer = await _context.Set<HabitLog>()
                            .Where(a => !a.IsDeleted)
                            .SingleOrDefaultAsync(exp);
            return answer;
        }

        public void Remove(HabitLog log)
        {
            log.IsDeleted = true;
            _context.Set<HabitLog>()
                .Update(log);
            _context.SaveChanges();
        }

        public HabitLog Update(HabitLog log)
        {
            log.IsDeleted = true;
            _context.Set<HabitLog>()
                .Update(log);
            return log;
        }
    }
}

using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Infrastructure.Context;

namespace Habit_Battles.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HabitBattlesContext _context;

        public UnitOfWork(HabitBattlesContext context)
        {
            _context = context;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

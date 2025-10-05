using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Habit_Battles.Infrastructure.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        public readonly HabitBattlesContext _context;
        public PurchaseRepository(HabitBattlesContext context)
        {
            _context = context;
        }
        public async Task<Purchases> AddAsync(Purchases purchases)
        {
            await _context.Set<Purchases>()
                .AddAsync(purchases);
            _context.SaveChanges();
            return purchases;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Purchases.AnyAsync(a => a.Id == id);
        }

        public async Task<ICollection<Purchases>> GetAllAsync()
        {
            return await _context.Set<Purchases>()
                .ToListAsync();
        }

        public async Task<ICollection<Purchases>> GetAllAsync(int userId)
        {
            return await _context.Set<Purchases>()
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Purchases> GetAsync(int id)
        {
            var answer = await _context.Set<Purchases>()
                        .Where(a => !a.IsDeleted && a.Id == id)
                        .SingleOrDefaultAsync();
            return answer;
        }

        public async Task<Purchases> GetAsync(Expression<Func<Purchases, bool>> exp)
        {
            var answer = await _context.Set<Purchases>()
                                .Where(a => !a.IsDeleted)
                                .SingleOrDefaultAsync(exp);
            return answer;
        }

        public void Remove(Purchases purchases)
        {
            purchases.IsDeleted = true;
            _context.Set<Purchases>()
                .Update(purchases);
            _context.SaveChanges();
        }

        public Purchases Update(Purchases purchases)
        {
            _context.Purchases.Update(purchases);
            _context.SaveChangesAsync();
            return purchases;
        }
    }
}

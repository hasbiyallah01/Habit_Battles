using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Habit_Battles.Infrastructure.Repositories
{
    public class ShopItemRepository : IShopItemRepository
    {
        private readonly HabitBattlesContext _context;
        public ShopItemRepository(HabitBattlesContext context)
        {
            _context = context;
        }
        public async Task<ShopItem> AddAsync(ShopItem shopItem)
        {
            await _context.Set<ShopItem>()
                .AddAsync(shopItem);
            _context.SaveChanges();
            return shopItem;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ShopItems.AnyAsync(a => a.Id == id);
        }

        public async Task<ICollection<ShopItem>> GetAllAsync()
        {
            return await _context.Set<ShopItem>()
                .ToListAsync();
        }

        public async Task<ShopItem> GetAsync(int id)
        {
            var answer = await _context.Set<ShopItem>()
                .Where(a => a.Id == id && !a.IsDeleted)
                .SingleOrDefaultAsync();
            return answer;
        }

        public async Task<ShopItem> GetAsync(Expression<Func<ShopItem, bool>> exp)
        {
            var answer = await _context.Set<ShopItem>()
                .Where(a => a.IsDeleted)
                .SingleOrDefaultAsync(exp);

            return answer;
        }

        public void Remove(ShopItem shopItem)
        {
            shopItem.IsDeleted = true;
            _context.Set<ShopItem>()
                .Update(shopItem);
            _context.SaveChangesAsync();
        }

        public ShopItem Update(ShopItem shopItem)
        {
            _context.ShopItems.Update(shopItem);
            return shopItem;
        }
    }
}

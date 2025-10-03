using Habit_Battles.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Habit_Battles.Core.Application.Interfaces.Repositories
{
    public interface IShopItemRepository
    {
        Task<ShopItem> AddAsync(ShopItem shopItem);
        Task<ShopItem> GetAsync(int id);
        Task<ShopItem> GetAsync(Expression<Func<ShopItem, bool>> exp);
        Task<ICollection<ShopItem>> GetAllAsync();
        void Remove(ShopItem shopItem);
        ShopItem Update(ShopItem shopItem);
        Task<bool> ExistsAsync(int id);
    }
}

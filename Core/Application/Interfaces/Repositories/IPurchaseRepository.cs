using Habit_Battles.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Habit_Battles.Core.Application.Interfaces.Repositories
{
    public interface IPurchaseRepository
    {
        Task<Purchases> AddAsync(Purchases purchases);
        Task<Purchases> GetAsync(int id);
        Task<Purchases> GetAsync(Expression<Func<Purchases, bool>> exp);
        Task<ICollection<Purchases>> GetAllAsync();
        void Remove(Purchases purchases);
        Purchases Update(Purchases purchases);
        Task<bool> ExistsAsync(int id);
    }
}

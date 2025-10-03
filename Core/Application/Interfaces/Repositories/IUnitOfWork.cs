namespace Habit_Battles.Core.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
    }
}

namespace Habit_Battles.Core.Domain.Entities
{
    public class HabitLog : Auditables
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int BattleId { get; set; }
        public Battle Battle { get; set; }
        public DateTime Date { get; set; }

        public int StreakCount { get; set; }
        public int CoinsEarned { get; set; }
    }
}

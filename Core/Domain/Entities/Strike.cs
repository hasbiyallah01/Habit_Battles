namespace Habit_Battles.Core.Domain.Entities
{
    public class Strike : Auditables
    {
        public int BattleId { get; set; }
        public Battle Battle { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public int StreakCount { get; set; }
        public int CoinsEarned { get; set; }    

    }
}

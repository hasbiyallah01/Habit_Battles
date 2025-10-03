namespace Habit_Battles.Core.Domain.Entities
{
    public class UserBattle : Auditables
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int BattleId { get; set; }
        public Battle Battle { get; set; }

        public int Streak { get; set; }
        public bool IsWinner { get; set; }
    }
}

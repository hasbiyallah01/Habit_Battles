namespace Habit_Battles.Models
{
    public class StrikeResponse
    {
        public int UserId { get; set; }
        public int BattleId { get; set; }
        public int CreatorHealth { get; set; }
        public int OpponentHealth { get; set; } 
        public int Streak { get; set; }
        public int CoinEarned { get; set; }
    }
}

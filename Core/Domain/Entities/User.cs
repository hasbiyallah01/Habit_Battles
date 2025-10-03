namespace Habit_Battles.Core.Domain.Entities
{
    public class User : Auditables
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int Coins { get; set; }
        public int Streaks { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public ICollection<UserBattle> Battles { get; set; } = new List<UserBattle>();
        public ICollection<Purchases> Purchases { get; set; } = new List<Purchases>();
        public ICollection<HabitLog> HabitLogs { get; set; } = new List<HabitLog>();
    }
}

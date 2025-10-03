using Habit_Battles.Core.Domain.Enums;

namespace Habit_Battles.Core.Domain.Entities
{
    public class Battle : Auditables
    {
        public string Habit {  get; set; }
        public int DurationDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MonsterHealth { get; set; } = 100;
        public Status Status { get; set; } = Status.Pending;

        public ICollection<UserBattle> Players { get; set; } = new List<UserBattle>();
        public ICollection<HabitLog> HabitLogs { get; set; } = new List<HabitLog>();
    }
}

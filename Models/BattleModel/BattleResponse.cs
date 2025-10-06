using Habit_Battles.Core.Domain.Enums;

namespace Habit_Battles.Models.BattleModel
{
    public class BattleResponse
    {
        public int Id { get; set; }
        public string RefId { get; set; }
        public string Habit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MonsterType MonsterType { get; set; }
        public int DurationDays { get; set; }
        public int CreatorHealth { get; set; }
        public int OpponentHealth { get; set; }
        public Status Status { get; set; }

        public List<PlayerResponse> Players { get; set; } = new List<PlayerResponse>();
    }

    public class PlayerResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int Streak { get; set; }
    }

    public class BattleAcceptResponse
    {
        public int Id { get; set; }
        public Status Status { get; set; }
    }

    public class EndBattleResponse
    {
        public int BatlleId { get; set; }
        public string Winner {  get; set; }
        public string Loser { get; set; }
        public int OpponentHealth { get; set; }
        public int CreatorHealth { get; set; }
        public int CoinsAwarded { get; set; }
    }
}

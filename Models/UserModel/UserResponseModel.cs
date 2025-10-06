using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Habit_Battles.Models.UserModel
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }
    }

    public class ProfileResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Coins { get; set; }
        public int Streaks { get; set; }
        public ICollection<HabitLog> Achievements { get; set; } = new List<HabitLog>();
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Streaks { get; set; }
        public int CoinBalance { get; set; }
        public IEnumerable<BattleSummaryDto> ActiveBattles { get; set; } = new List<BattleSummaryDto>();
    }

    public class BattleSummaryDto
    {
        public int BattleId { get; set; }
        public string Habit { get; set; }
        public string OpponentName { get; set; }
        public int CreatorHealth { get; set; }
        public MonsterType MonsterType { get; set; }
        public int OpponentHealth { get; set; }
        public Status Status { get; set; }
    }

}

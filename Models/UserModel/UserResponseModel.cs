using Habit_Battles.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Habit_Battles.Models.UserModel
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }
    }

    public class LoginResponse
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
}

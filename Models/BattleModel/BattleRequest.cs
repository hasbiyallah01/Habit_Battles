namespace Habit_Battles.Models.BattleModel
{
    public class BattleRequest
    {
        public string Habit {  get; set; }
        public int duration { get; set; }
        public int opponentId { get; set; }
    }
}

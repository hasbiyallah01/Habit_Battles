namespace Habit_Battles.Core.Domain.Entities
{
    public class ShopItem : Auditables
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public Type Type { get; set; }

        public ICollection<Purchases> Purchases { get; set; } = new List<Purchases>();
    }
}

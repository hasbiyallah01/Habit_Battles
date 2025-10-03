namespace Habit_Battles.Core.Domain.Entities
{
    public class Purchases : Auditables
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ShopItemId { get; set; }
        public ShopItem ShopItem { get; set; }
        public DateTime PurchasedAt { get; set; }
    }
}

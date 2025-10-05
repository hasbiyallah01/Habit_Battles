using Habit_Battles.Core.Domain.Enums;

namespace Habit_Battles.Models.ShopModel
{
    public class ShopItemResponse
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public MonsterType Type { get; set; }
    }
    public class ShopItemRequest
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public MonsterType Type { get; set; }
    }

    public class UpdateShopItemRequest
    {
        public string? Name { get; set; }
        public int? Cost { get; set; }
        public MonsterType? Type { get; set; }
    }
    public class PurchaseResponse
    {
        public string Message { get; set; }
        public int CoinsRemaining { get; set; }
        public List<string> Inventory { get; set; }
    }
}

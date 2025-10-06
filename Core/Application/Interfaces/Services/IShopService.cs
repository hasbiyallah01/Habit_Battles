using Habit_Battles.Models;
using Habit_Battles.Models.ShopModel;

namespace Habit_Battles.Core.Application.Interfaces.Services
{
    public interface IShopService
    {
        Task<BaseResponse<IEnumerable<ShopItemResponse>>> GetShopItemAsync();
        Task<BaseResponse<PurchaseResponse>> PurchaseItemAsync(PurchaseRequest purchaseRequest);
        Task<BaseResponse<ShopItemResponse>> GetShopItemByIdAsync(int itemId);
        Task<BaseResponse<bool>> DeleteShopItemAsync(int itemId);
        Task<BaseResponse<ShopItemResponse>> UpdateShopItemAsync(int itemId, UpdateShopItemRequest request);
        Task<BaseResponse<ShopItemResponse>> AddShopItemAsync(int userId, ShopItemRequest request);
    }
}

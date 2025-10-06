using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Application.Interfaces.Services;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Infrastructure.Repositories;
using Habit_Battles.Models;
using Habit_Battles.Models.ShopModel;
using MimeKit.Cryptography;

namespace Habit_Battles.Core.Application.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopItemRepository _shopItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ShopService(IShopItemRepository shopItemRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _shopItemRepository = shopItemRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseResponse<IEnumerable<ShopItemResponse>>> GetShopItemAsync()
        {
            var items = await _shopItemRepository.GetAllAsync();
            if(items == null)
            {
                return new BaseResponse<IEnumerable<ShopItemResponse>> 
                {
                    IsSuccessful = false,
                    Message = "No shop item"
                };
            }

            var itemResponses = items.Select(item => new ShopItemResponse
            {
                ItemId = item.Id,
                Name = item.Name,
                Cost = item.Cost
            }).ToList();

            return new BaseResponse<IEnumerable<ShopItemResponse>>
            {
                IsSuccessful = true,
                Message = "Shop items retrieved successfully",
                Value = itemResponses
            };
        }
        public async Task<BaseResponse<PurchaseResponse>> PurchaseItemAsync(PurchaseRequest purchaseRequest)
        {
            var user = await _userRepository.GetAsync(purchaseRequest.UserId);
            if (user == null)
            {
                return new BaseResponse<PurchaseResponse>
                {
                    IsSuccessful = false,
                    Message = "User not found."
                };
            }

            var item = await _shopItemRepository.GetAsync(purchaseRequest.ItemId);
            if (item == null)
            {
                return new BaseResponse<PurchaseResponse>
                {
                    IsSuccessful = false,
                    Message = "Item not found."
                };
            }

            if (user.Coins < item.Cost)
            {
                return new BaseResponse<PurchaseResponse>
                {
                    IsSuccessful = false,
                    Message = "Insufficient coins to purchase this item."
                };
            }

            user.Coins -= item.Cost;
            user.Purchases ??= new List<Purchases>();

            user.Purchases.Add(new Purchases
            {
                UserId = purchaseRequest.UserId,
                ShopItem = item,
                ShopItemId = item.Id,
                PurchasedAt = DateTime.UtcNow
            });

            _userRepository.Update(user);
            await _unitOfWork.SaveAsync();
            return new BaseResponse<PurchaseResponse>
            {
                IsSuccessful = true,
                Message = $"Purchase successful! You bought {item.Name}.",
                Value = new PurchaseResponse
                {
                    CoinsRemaining = user.Coins,
                    Inventory = user.Purchases.Select(p => p.ShopItem.Name).ToList()
                }
            };
        }
        public async Task<BaseResponse<ShopItemResponse>> AddShopItemAsync(int userId, ShopItemRequest request)
        {
            var user = _userRepository.GetAsync(userId);
            if(user == null)
            {
                return new BaseResponse<ShopItemResponse>
                {
                    IsSuccessful = false,
                    Message = "You need to be authenticated to add Item to the shop",
                };
            }
            var newItem = new ShopItem
            {
                Name = request.Name,
                Cost = request.Cost,
                Type = request.Type,
                CreatedBy = user.Id.ToString(),
            };

            var addedItem = await _shopItemRepository.AddAsync(newItem);
            await _unitOfWork.SaveAsync();
            return new BaseResponse<ShopItemResponse>
            {
                IsSuccessful = true,
                Message = "Shop item added successfully",
                Value = new ShopItemResponse
                {
                    ItemId = addedItem.Id,
                    Name = addedItem.Name,
                    Cost = addedItem.Cost,
                    Type = addedItem.Type
                }
            };
        }
        public async Task<BaseResponse<ShopItemResponse>> UpdateShopItemAsync(int itemId, UpdateShopItemRequest request)
        {
            var existingItem = await _shopItemRepository.GetAsync(itemId);
            if (existingItem == null)
            {
                return new BaseResponse<ShopItemResponse>
                {
                    IsSuccessful = false,
                    Message = "Item not found."
                };
            }

            existingItem.Name = request.Name ?? existingItem.Name;
            existingItem.Cost = request.Cost ?? existingItem.Cost;
            existingItem.Type = request.Type ?? existingItem.Type;

            _shopItemRepository.Update(existingItem);
            await _unitOfWork.SaveAsync();
            return new BaseResponse<ShopItemResponse>
            {
                IsSuccessful = true,
                Message = "Shop item updated successfully",
                Value = new ShopItemResponse
                {
                    ItemId = existingItem.Id,
                    Name = existingItem.Name,
                    Cost = existingItem.Cost,
                    Type = existingItem.Type
                }
            };
        }
        public async Task<BaseResponse<bool>> DeleteShopItemAsync(int itemId)
        {
            var item = await _shopItemRepository.GetAsync(itemId);
            if (item == null)
            {
                return new BaseResponse<bool>
                {
                    IsSuccessful = false,
                    Message = "Item not found."
                };
            }

            _shopItemRepository.Remove(item);

            return new BaseResponse<bool>
            {
                IsSuccessful = true,
                Message = "Shop item deleted successfully.",
                Value = true
            };
        }
        public async Task<BaseResponse<ShopItemResponse>> GetShopItemByIdAsync(int itemId)
        {
            var item = await _shopItemRepository.GetAsync(itemId);
            if (item == null)
            {
                return new BaseResponse<ShopItemResponse>
                {
                    IsSuccessful = false,
                    Message = "Item not found."
                };
            }

            return new BaseResponse<ShopItemResponse>
            {
                IsSuccessful = true,
                Message = "Shop item retrieved successfully.",
                Value = new ShopItemResponse
                {
                    ItemId = item.Id,
                    Name = item.Name,
                    Cost = item.Cost,
                    Type = item.Type
                }
            };
        }
        
    }
}

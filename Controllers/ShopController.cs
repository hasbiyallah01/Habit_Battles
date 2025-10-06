using Habit_Battles.Core.Application.Interfaces.Services;
using Habit_Battles.Models.ShopModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Habit_Battles.Controllers
{
    [Route("api/shop")]
    [ApiController]
    public class ShopController : ControllerBase
    { 
        private readonly IShopService _shopService;
        private readonly IUserService _userService;
        public ShopController(IShopService shopService, IUserService userService)
        {
            _shopService = shopService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var result = await _shopService.GetShopItemAsync();
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetItemById(int itemId)
        {
            var result = await _shopService.GetShopItemByIdAsync(itemId);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] ShopItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GetCurrentUserAsync();
            if(user == null)
            {
                return BadRequest("User Unauthorized");
            }
            var result = await _shopService.AddShopItemAsync(user.Value.Id, request);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateItem(int itemId, [FromBody] UpdateShopItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _shopService.UpdateShopItemAsync(itemId, request);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            var result = await _shopService.DeleteShopItemAsync(itemId);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseItem([FromBody] PurchaseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _shopService.PurchaseItemAsync(request);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }
    }
}

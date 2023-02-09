using System.Security.Claims;
using Mango.Web.Models.Dto;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly IShoppingCartService _shoppingCartService;

    public CartController(IProductService productService, IShoppingCartService shoppingCartService)
    {
        _productService = productService;
        _shoppingCartService = shoppingCartService;
    }
    
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }
    
    public async Task<IActionResult> Remove(long cartDetailId)
    {
        var result = await _shoppingCartService.RemoveFromCartAsync<ResponseDto>(cartDetailId);
        if (result is not null & result.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }
        
        return RedirectToAction(nameof(CartIndex));
    }

    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;
        var result = await _shoppingCartService.GetCartByUserIdAsync<ResponseDto>(userId);
        CartDto cartDto = new();
        if (result is not null & result.IsSuccess)
        {
            cartDto = JsonConvert.DeserializeObject<CartDto>(result.Result.ToString());
        }

        if (cartDto.CartHeader is not null)
        {
            foreach (var detail in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
            }
        }

        return cartDto;
    }
}
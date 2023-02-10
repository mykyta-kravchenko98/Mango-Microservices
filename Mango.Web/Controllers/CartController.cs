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
    private readonly ICouponService _couponService;

    public CartController(IProductService productService,
        IShoppingCartService shoppingCartService, ICouponService couponService)
    {
        _productService = productService;
        _shoppingCartService = shoppingCartService;
        _couponService = couponService;
    }
    
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }
    
    [HttpPost]
    [ActionName("ApplyCoupon")]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        var result = await _shoppingCartService.ApplyCoupon<ResponseDto>(cartDto);
        if (result is not null & result.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }
        
        return RedirectToAction(nameof(CartIndex));
    }
    
    [HttpPost]
    [ActionName("RemoveCoupon")]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        var result = await _shoppingCartService.RemoveCoupon<ResponseDto>(cartDto.CartHeader.UserId);
        if (result is not null & result.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }
        
        return RedirectToAction(nameof(CartIndex));
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

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
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
            if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            {
                var couponDto = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode);
                if (couponDto is not null && couponDto.IsSuccess)
                {
                    var couponObj = JsonConvert.DeserializeObject<CouponDto>(couponDto.Result.ToString());
                    cartDto.CartHeader.DiscountTotal = couponObj.DiscountAmount;
                }
            }
            foreach (var detail in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
            }

            cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
        }

        return cartDto;
    }
}
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[Authorize]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    protected ResponseDto _response;

    public CartController(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
        _response = new ResponseDto();
    }

    [HttpGet("GetCart/{userId}")]
    public async Task<ResponseDto> GetCart(string userId)
    {
        try
        {
            var cartDto = await _cartRepository.GetCartByUserId(userId);
            _response.Result = cartDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpPost("AddCart")]
    public async Task<ResponseDto> AddCart([FromBody]CartDto cartDto)
    {
        try
        {
            var result = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpPost("UpdateCart")]
    public async Task<ResponseDto> UpdateCart([FromBody]CartDto cartDto)
    {
        try
        {
            var result = await _cartRepository.CreateUpdateCart(cartDto);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpPost("ApplyCoupon")]
    public async Task<ResponseDto> ApplyCoupon([FromBody]CartDto cartDto)
    {
        try
        {
            var result = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpDelete("DeleteCart/{cartDetailId}")]
    public async Task<ResponseDto> DeleteCart(long cartDetailId)
    {
        try
        {
            var result = await _cartRepository.RemoveFromCart(cartDetailId);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpDelete("ClearCart/{userId}")]
    public async Task<ResponseDto> ClearCart(string userId)
    {
        try
        {
            var result = await _cartRepository.ClearCart(userId);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpDelete("RemoveCoupon/{userId}")]
    public async Task<ResponseDto> RemoveCoupon(string userId)
    {
        try
        {
            var result = await _cartRepository.RemoveCoupon(userId);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
}
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : Controller
{
    private readonly ICartRepository _cartRepository;
    protected ResponseDto _response;

    public CartController(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
        _response = new ResponseDto();
    }

    [HttpGet("GetCart/{userId}")]
    public async Task<object> GetCart(string userId)
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
    public async Task<object> AddCart(CartDto cartDto)
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
    public async Task<object> UpdateCart(CartDto cartDto)
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
    
    [HttpDelete("DeleteCart")]
    public async Task<object> DeleteCart([FromBody] long cartDetailId)
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
    
    [HttpDelete("ClearCart")]
    public async Task<object> ClearCart([FromBody] string userId)
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
}
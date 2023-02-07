using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Repository.Interfaces;

public interface ICartRepository
{
    Task<CartDto> GetCartByUserId(string userId);
    Task<CartDto> CreateUpdateCart(CartDto cartDto);
    Task<bool> RemoveFromCart(long cartDetailId);
    Task<bool> ClearCart(string userId);
}
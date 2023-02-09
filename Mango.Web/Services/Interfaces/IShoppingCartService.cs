using Mango.Web.Models.Dto;

namespace Mango.Web.Services.Interfaces;

public interface IShoppingCartService
{
    Task<T> GetCartByUserIdAsync<T>(string userId);
    Task<T> AddCartAsync<T>(CartDto cartDto);
    Task<T> UpdateCartAsync<T>(CartDto cartDto);
    Task<T> RemoveFromCartAsync<T>(long cartDetailId);
    Task<T> ClearCartByUserIdAsync<T>(string userId);
    Task<T> ApplyCoupon<T>(CartDto cartDto);
    Task<T> RemoveCoupon<T>(string userId);
}
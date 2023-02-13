using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Repository.Interfaces;

public interface ICouponRepository
{
    Task<CouponDto> GetCoupon(string couponCode);
}
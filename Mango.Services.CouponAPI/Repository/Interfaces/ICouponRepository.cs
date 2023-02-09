using Mango.Services.CouponAPI.Models.Dtos;

namespace Mango.Services.CouponAPI.Repository.Interfaces;

public interface ICouponRepository
{
    Task<CouponDto> GetCouponByCodeAsync(string couponCode);
}
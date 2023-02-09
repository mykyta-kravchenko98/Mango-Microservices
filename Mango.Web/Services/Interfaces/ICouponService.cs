namespace Mango.Web.Services.Interfaces;

public interface ICouponService
{
    Task<T> GetCoupon<T>(string couponCode);
}
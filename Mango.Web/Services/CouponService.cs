using Mango.Web.Models;
using Mango.Web.Services.Interfaces;

namespace Mango.Web.Services;

public class CouponService : BaseService, ICouponService
{
    private readonly string _couponApiUrl;
    private const string CouponApiControllerPath = "api/coupon";
    
    public CouponService(IHttpClientFactory httpClient, IHttpContextAccessor accessor) : base(httpClient, accessor)
    {
        _couponApiUrl = $"{SD.CouponApiBase}/{CouponApiControllerPath}";
    }
    
    public async Task<T> GetCoupon<T>(string couponCode)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{_couponApiUrl}/DiscountForCode/{couponCode}"
        });
    }
}
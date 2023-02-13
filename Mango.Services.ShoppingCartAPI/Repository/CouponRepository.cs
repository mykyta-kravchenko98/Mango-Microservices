using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Repository.Interfaces;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace Mango.Services.ShoppingCartAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient _httpClient;

    public CouponRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CouponDto> GetCoupon(string couponCode)
    {
        var response = await _httpClient.GetAsync($"/api/coupon/{couponCode}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDto>(resp.Result.ToString());
        }

        return new CouponDto();
    }
}
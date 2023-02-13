using System.Net.Http.Headers;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace Mango.Services.ShoppingCartAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _accessor;

    public CouponRepository(HttpClient httpClient, IHttpContextAccessor accessor)
    {
        _httpClient = httpClient;
        _accessor = accessor;
    }

    public async Task<CouponDto> GetCoupon(string couponCode)
    {
        var accessToken = await _accessor.HttpContext.GetTokenAsync("access_token");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await _httpClient.GetAsync($"/api/coupon/DiscountForCode/{couponCode}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDto>(resp.Result.ToString());
        }

        return new CouponDto();
    }
}
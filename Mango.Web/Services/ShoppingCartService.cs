using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.Interfaces;

namespace Mango.Web.Services;

public class ShoppingCartService: BaseService, IShoppingCartService
{
    private readonly string _shoppingCartApiUrl;
    private const string ShoppingCartApiControllerPath = "api/cart";
    
    public ShoppingCartService(IHttpClientFactory httpClient, IHttpContextAccessor accessor) : base(httpClient, accessor)
    {
        _shoppingCartApiUrl = $"{SD.ShoppingCartApiBase}/{ShoppingCartApiControllerPath}";
    }

    public async Task<T> GetCartByUserIdAsync<T>(string userId)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{_shoppingCartApiUrl}/GetCart/{userId}"
        });
    }

    public async Task<T> AddCartAsync<T>(CartDto cartDto)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = $"{_shoppingCartApiUrl}/AddCart"
        });
    }

    public async Task<T> UpdateCartAsync<T>(CartDto cartDto)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = $"{_shoppingCartApiUrl}/UpdateCart"
        });
    }

    public async Task<T> RemoveFromCartAsync<T>(long cartDetailId)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{_shoppingCartApiUrl}/DeleteCart/{cartDetailId}"
        });
    }

    public async Task<T> ClearCartByUserIdAsync<T>(string userId)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{_shoppingCartApiUrl}/ClearCart/{userId}"
        });
    }

    public async Task<T> ApplyCoupon<T>(CartDto cartDto)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = $"{_shoppingCartApiUrl}/ApplyCoupon"
        });
    }

    public async Task<T> RemoveCoupon<T>(string userId)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{_shoppingCartApiUrl}/RemoveCoupon/{userId}"
        });
    }
}
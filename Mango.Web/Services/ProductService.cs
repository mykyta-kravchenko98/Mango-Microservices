using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.Interfaces;

namespace Mango.Web.Services;

public class ProductService : BaseService, IProductService
{
    private readonly string _productApiUrl;
    private const string ProductApiControllerPath = "api/products";

    public ProductService(IHttpClientFactory httpClient, IHttpContextAccessor contextAccessor) : base(httpClient, contextAccessor)
    {
        _productApiUrl = $"{SD.ProductApiBase}/{ProductApiControllerPath}";
    }

    public async Task<T> GetAllProductsAsync<T>()
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = _productApiUrl
        });
    }

    public async Task<T> GetProductsByIdAsync<T>(long id)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{_productApiUrl}/{id}"
        });
    }

    public async Task<T> CreateProductAsync<T>(ProductDto product)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = product,
            Url = _productApiUrl
        });
    }

    public async Task<T> UpdateProductAsync<T>(ProductDto product)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = product,
            Url = _productApiUrl
        });
    }

    public async Task<T> DeleteProductAsync<T>(long id)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{_productApiUrl}/{id}"
        });
    }
}
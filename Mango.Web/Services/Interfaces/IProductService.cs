using Mango.Web.Models.Dtos;

namespace Mango.Web.Services.Interfaces;

public interface IProductService : IBaseService
{
    Task<T> GetAllProductsAsync<T>();
    Task<T> GetProductsByIdAsync<T>(long id);
    Task<T> CreateProductAsync<T>(ProductDto product);
    Task<T> UpdateProductAsync<T>(ProductDto product);
    Task<T> DeleteProductAsync<T>(long id);
}
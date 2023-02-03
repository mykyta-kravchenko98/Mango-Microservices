using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI.Repository.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<ProductDto>> GetProducts();
    Task<ProductDto> GetProductById(long productId);
    Task<ProductDto> CreateUpdateProduct(ProductDto productDto);
    Task<bool> DeleteProduct(long productId);
}
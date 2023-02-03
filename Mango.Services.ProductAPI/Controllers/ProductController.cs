using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[Route("api/products")]
public class ProductController : ControllerBase
{
    protected ResponseDto _response;

    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        this._response = new ResponseDto();
    }

    [HttpGet]
    public async Task<ResponseDto> Get()
    {
        try
        {
            var productDtos = await _productRepository.GetProducts();
            _response.Result = productDtos;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>()
            {
                ex.ToString()
            };
        }

        return _response;
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<ResponseDto> Get(long id)
    {
        try
        {
            var productDto = await _productRepository.GetProductById(id);
            _response.Result = productDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>()
            {
                ex.ToString()
            };
        }

        return _response;
    }
    
    [HttpPost]
    public async Task<ResponseDto> Post([FromBody]ProductDto productDto)
    {
        try
        {
            var result = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>()
            {
                ex.ToString()
            };
        }

        return _response;
    }
    
    [HttpPut]
    public async Task<ResponseDto> Put([FromBody]ProductDto productDto)
    {
        try
        {
            var result = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>()
            {
                ex.ToString()
            };
        }

        return _response;
    }
    
    [HttpDelete]
    [Route("{id}")]
    public async Task<ResponseDto> Delete(long id)
    {
        try
        {
            var isSuccess = await _productRepository.DeleteProduct(id);
            _response.Result = isSuccess;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>()
            {
                ex.ToString()
            };
        }

        return _response;
    }
}
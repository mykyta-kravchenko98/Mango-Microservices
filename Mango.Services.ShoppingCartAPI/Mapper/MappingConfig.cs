using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Mapper;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<ProductDto, Product>().ReverseMap();
            config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            config.CreateMap<CartDetailDto, CartDetail>().ReverseMap();
            config.CreateMap<CartDto, Cart>().ReverseMap();
        });

        return mappingConfig;
    }
}
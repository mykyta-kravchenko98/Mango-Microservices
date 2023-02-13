using AutoMapper;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;

namespace Mango.Services.OrderAPI.Mapper;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CheckoutHeaderDto, OrderHeader>()
                .ForMember(dest => dest.OrderTime, opt => opt.MapFrom(s => DateTime.Now))
                .ForMember(dest => dest.StatusOfPayment, opt => opt.MapFrom(s => PaymentStatus.New))
                .ForMember(dest => dest.CartTotalItems, opt => opt.MapFrom(s => s.CartDetails.Sum(cd => cd.Count)));

            config.CreateMap<CartDetailDto, OrderDetail>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(s => s.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(s => s.Product.Price))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(s => s.Count))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(s => s.ProductId));
        });

        return mappingConfig;
    }
}
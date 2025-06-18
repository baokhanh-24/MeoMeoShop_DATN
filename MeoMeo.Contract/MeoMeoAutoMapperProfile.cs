using AutoMapper;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Contract
{
    public class MeoMeoAutoMapperProfile : Profile
    {
        public MeoMeoAutoMapperProfile()
        {
            CreateMap<CreateOrUpdateProductDTO, Product>();
            CreateMap<CartDTO, Cart>();
            CreateMap<CartDetailDTO, CartDetail>();
            CreateMap<ImageDTO, Image>();
            CreateMap<ColourDTO, Colour>();
            CreateMap<ProductDetaillColourDTO, ProductDetailColour>();
            CreateMap<SizeDTO,  Size>();
            CreateMap<ProductDetaillSizeDTO, ProductDetailSize>();
            CreateMap<CreateOrUpdateOrderDTO, Order>();
            CreateMap<CreateOrUpdateOrderDetailDTO, OrderDetail>();
            CreateMap<CreateOrUpdateDeliveryAddressDTO, DeliveryAddress>();
            CreateMap<CreateOrUpdateProvinceDTO, Province>();
        }

    }
}

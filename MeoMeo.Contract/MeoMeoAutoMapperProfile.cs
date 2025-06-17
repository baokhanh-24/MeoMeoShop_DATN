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
            CreateMap<CreateOrUpdateOrderDTO, Order>();
            CreateMap<CreateOrUpdateOrderDetailDTO, OrderDetail>();
            CreateMap<CreateOrUpdateDeliveryAddressDTO, DeliveryAddress>();
            CreateMap<CreateOrUpdateProvinceDTO, Province>();
        }

    }
}

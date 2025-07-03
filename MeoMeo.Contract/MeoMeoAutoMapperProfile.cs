using AutoMapper;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Contract
{
    public class MeoMeoAutoMapperProfile : Profile
    {
        public MeoMeoAutoMapperProfile()
        {
            CreateMap<CreateOrUpdateProductDTO, Product>();
            CreateMap<CreateOrUpdateProductDetailDTO, ProductDetail>();
            CreateMap<Product, ProductReponseDTO>();
            CreateMap<ProductDetail, CreateOrUpdateProductDetailResponseDTO>();
            CreateMap<ProductDetail, ProductDetailDTO>();
            CreateMap<CreateOrUpdateProductDetailDTO, ProductDetail>();
            CreateMap<CartDTO, Cart>();
            CreateMap<CartDetailDTO, CartDetail>();
            CreateMap<ImageDTO, Image>();
            CreateMap<ColourDTO, Colour>();
            CreateMap<ProductDetaillColourDTO, ProductDetailColour>();
            CreateMap<SizeDTO, Size>();
            CreateMap<ProductDetaillSizeDTO, ProductDetailSize>();
            CreateMap<CreateOrUpdatePromotionDTO, Promotion>();
            CreateMap<CreateOrUpdatePromotionDetailDTO, PromotionDetail>();
            CreateMap<CreateOrUpdateVoucherDTO, Voucher>();
            CreateMap<CreateOrUpdateUserDTO, User>();
            CreateMap<CreateOrUpdateEmployeeDTO, Employee>();
            CreateMap<CreateOrUpdateCustomerDTO, Customers>();
            CreateMap<CreateOrUpdateResetPasswordHistoryDTO, ResetPasswordHistory>();
            CreateMap<CreateOrUpdateBankDTO, Bank>();
            CreateMap<CreateOrUpdateCustomersBankDTO, CustomersBank>();
            CreateMap<CartDTO, Cart>();
            CreateMap<CartDetailDTO, CartDetail>();
            CreateMap<ImageDTO, Image>();
            CreateMap<ColourDTO, Colour>();
            CreateMap<ProductDetaillColourDTO, ProductDetailColour>();
            CreateMap<SizeDTO, Size>();
            CreateMap<ProductDetaillSizeDTO, ProductDetailSize>();
            CreateMap<CreateOrUpdateOrderDTO, Order>();
            CreateMap<CreateOrUpdateOrderDetailDTO, OrderDetail>();
            CreateMap<CreateOrUpdateDeliveryAddressDTO, DeliveryAddress>();
            CreateMap<CreateOrUpdateProvinceDTO, Province>();
            CreateMap<CreateOrUpdateMaterialDTO, Material>();

            CreateMap<Voucher, CreateOrUpdateVoucherResponseDTO>();
            CreateMap<Promotion, CreateOrUpdatePromotionResponseDTO>();
            CreateMap<PromotionDetail, CreateOrUpdatePromotionDetailResponseDTO>();
            CreateMap<Bank, CreateOrUpdateBankResponseDTO>();
            CreateMap<User, CreateOrUpdateUserResponseDTO>();
            CreateMap<Employee, CreateOrUpdateEmployeeResponseDTO>();

            CreateMap<CreateOrUpdateDistrictDTO, District>();
            CreateMap<Order, CreateOrUpdateOrderResponse>();

            CreateMap<Customers, CreateOrUpdateCustomerResponse>();
            CreateMap<Customers, CustomerDTO>();

            CreateMap<Bank, BankDTO>();
            CreateMap<Bank,BankDTO>();
            CreateMap<Material, CreateOrUpdateMaterialDTO>();
            CreateMap<Material, CreateOrUpdateMaterialResponse>();

            CreateMap<Size, SizeDTO>();
        }

    }
}

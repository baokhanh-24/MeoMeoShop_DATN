using AutoMapper;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Contract.DTOs.OrderDetail;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Contract.DTOs.Size;
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
            CreateMap<CreateOrUpdateOrderDetailDTO, OrderDetail>();
            CreateMap<CreateOrUpdateDeliveryAddressDTO, DeliveryAddress>();
            CreateMap<CreateOrUpdateProvinceDTO, Province>();
            CreateMap<InventoryBatch, InventoryBatchDTO>();
            CreateMap<InventoryBatchDTO, InventoryBatch>();
            CreateMap<InventoryBatch, InventoryBatchResponseDTO>();
            CreateMap<InventoryBatchResponseDTO, InventoryBatch>();
            CreateMap<CreateOrUpdateMaterialDTO, Material>();
            CreateMap<Voucher, CreateOrUpdateVoucherResponseDTO>();
            CreateMap<Promotion, CreateOrUpdatePromotionResponseDTO>();
            CreateMap<PromotionDetail, CreateOrUpdatePromotionDetailResponseDTO>();
            CreateMap<Bank, CreateOrUpdateBankResponseDTO>();
            CreateMap<User, CreateOrUpdateUserResponseDTO>();
            CreateMap<Employee, CreateOrUpdateEmployeeResponseDTO>();
            CreateMap<CreateOrUpdateDistrictDTO, District>();
            CreateMap<Customers, CreateOrUpdateCustomerResponse>();
            CreateMap<Customers, CustomerDTO>();
            CreateMap<Bank, BankDTO>();
            CreateMap<Bank,BankDTO>();
            CreateMap<Material, CreateOrUpdateMaterialDTO>();
            CreateMap<Material, CreateOrUpdateMaterialResponse>();
            CreateMap<Size, SizeDTO>();
            CreateMap<Employee, CreateOrUpdateEmployeeDTO>();
            CreateMap<CreateOrUpdateEmployeeDTO, Employee>();
            CreateMap<CreateOrUpdateEmployeeResponseDTO, Employee>();
            CreateMap<Employee, CreateOrUpdateEmployeeResponseDTO>();
            CreateMap<Order,OrderDTO>();
            CreateMap<OrderDetail,OrderDetailDTO>();
        }

    }
}

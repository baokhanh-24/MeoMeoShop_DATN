using AutoMapper;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Contract.DTOs.OrderDetail;
using MeoMeo.Contract.DTOs.InventoryBatch;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Contract.DTOs.PromotionDetail;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Contract.DTOs.SystemConfig;
using MeoMeo.Contract.Extensions;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Contract
{
    public class MeoMeoAutoMapperProfile : Profile
    {
        public MeoMeoAutoMapperProfile()
        {
            // Product mappings
            CreateMap<CreateOrUpdateProductDTO, Product>().IgnoreAllNonExisting();
            // ProductDetail mappings
            CreateMap<CreateOrUpdateProductDetailDTO, ProductDetail>();
            CreateMap<ProductDetail, CreateOrUpdateProductDetailResponseDTO>();
            CreateMap<ProductDetail, ProductDetailDTO>();
            CreateMap<ProductDetailGrid,ProductDetail>();
            CreateMap<Product,CreateOrUpdateProductDTO>();
            // Legacy mappings for backward compatibility
            CreateMap<CartDTO, Cart>();
            CreateMap<CartDetailDTO, CartDetail>();
            CreateMap<ImageDTO, Image>();
            CreateMap<ColourDTO, Colour>();
            CreateMap<SizeDTO, Size>();
            CreateMap<CreateOrUpdatePromotionDTO, Promotion>();
            CreateMap<Promotion, CreateOrUpdatePromotionDTO>();
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
            CreateMap<SizeDTO, Size>();
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
            CreateMap<PromotionDetail, CreateOrUpdatePromotionDetailDTO>();
            CreateMap<CreateOrUpdatePromotionDetailDTO, PromotionDetail>();
            CreateMap<Bank, CreateOrUpdateBankResponseDTO>();
            CreateMap<User, CreateOrUpdateUserResponseDTO>();
            CreateMap<User, UserDTO>();
            CreateMap<Employee, CreateOrUpdateEmployeeResponseDTO>();
            CreateMap<CreateOrUpdateDistrictDTO, District>();
            CreateMap<Order, CreateOrUpdateOrderResponse>();
            CreateMap<Customers,CreateOrUpdateCustomerResponse>();
            CreateMap<Customers,CustomerDTO>();
            CreateMap<Brand, BrandDTO>();
            CreateMap<Brand, CreateOrUpdateBrandResponseDTO>();
            CreateMap<Season, SeasonDTO>();
            CreateMap<Season, CreateOrUpdateSeasonResponseDTO>();
            CreateMap<CreateOrUpdateSeasonDTO, Season>();
            CreateMap<Customers, CreateOrUpdateCustomerResponse>();
            CreateMap<Customers, CustomerDTO>();
            CreateMap<Bank,BankDTO>();
            CreateMap<Material, CreateOrUpdateMaterialDTO>();
            CreateMap<Material, CreateOrUpdateMaterialResponse>();
            CreateMap<Size, SizeDTO>();
            CreateMap<Voucher, VoucherDTO>();
            CreateMap<Employee, CreateOrUpdateEmployeeDTO>();
            CreateMap<CreateOrUpdateEmployeeDTO, Employee>();
            CreateMap<CreateOrUpdateEmployeeResponseDTO, Employee>();
            CreateMap<Employee, CreateOrUpdateEmployeeResponseDTO>();
            CreateMap<Order,OrderDTO>();
            CreateMap<OrderDetail,OrderDetailDTO>();
            CreateMap<SystemConfig, SystemConfigDTO>();
            CreateMap<CreateOrUpdateSystemConfigDTO, SystemConfig>();
            CreateMap<CreateOrUpdateSystemConfigResponseDTO, SystemConfig>();
            CreateMap<CreateOrUpdateSystemConfigResponseDTO, SystemConfigDTO>();
            CreateMap<CreateOrUpdateSystemConfigDTO, SystemConfigDTO>();
            CreateMap<SystemConfig, CreateOrUpdateSystemConfigResponseDTO>();
            CreateMap<SystemConfig, CreateOrUpdateSystemConfigDTO>();
            CreateMap<SystemConfigDTO, CreateOrUpdateSystemConfigResponseDTO>();
            CreateMap<SystemConfigDTO, CreateOrUpdateSystemConfigDTO>();
            CreateMap<SystemConfigDTO, SystemConfig>();
            CreateMap<User,UserDTO>();
            CreateMap<ProductDetail,ProductDetailGrid>().IgnoreAllNonExisting();
            CreateMap<Image, ProductMediaUpload>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.URL))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ContentType,
                    opt => opt.MapFrom(src => src.Type == 0 ? "image/jpeg" : "video/mp4"));
            // Category mappings
            CreateMap<CategoryDTO, Category>();
            CreateMap<Category, CategoryResponseDTO>();
            CreateMap<Category, CategoryDTO>();
        }
    }
}

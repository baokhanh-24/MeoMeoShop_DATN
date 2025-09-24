using AutoMapper;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Contract.DTOs.OrderDetail;
using MeoMeo.Contract.DTOs.InventoryBatch;
using MeoMeo.Contract.DTOs.ImportBatch;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Contract.DTOs.ProductReview;
using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Contract.DTOs.PromotionDetail;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Contract.DTOs.Payment;
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
            CreateMap<ProductDetailGrid, ProductDetail>();
            CreateMap<Product, CreateOrUpdateProductDTO>();
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
            CreateMap<CartDTO, Cart>();
            CreateMap<CartDetailDTO, CartDetail>();
            CreateMap<ImageDTO, Image>();
            CreateMap<Colour, ColourDTO>();
            CreateMap<SizeDTO, Size>();
            CreateMap<CreateOrUpdateOrderDetailDTO, OrderDetail>();
            CreateMap<CreateOrUpdateDeliveryAddressDTO, DeliveryAddress>();
            CreateMap<DeliveryAddress, DeliveryAddressDTO>();
            CreateMap<InventoryBatch, InventoryBatchDTO>();
            CreateMap<InventoryBatchDTO, InventoryBatch>();
            CreateMap<InventoryBatch, InventoryBatchResponseDTO>();
            CreateMap<InventoryBatchResponseDTO, InventoryBatch>();

            // ImportBatch mappings
            CreateMap<ImportBatch, ImportBatchDTO>();
            CreateMap<ImportBatchDTO, ImportBatch>();
            CreateMap<ImportBatch, ImportBatchResponseDTO>();
            CreateMap<ImportBatchResponseDTO, ImportBatch>();
            CreateMap<CreateOrUpdateMaterialDTO, Material>();
            CreateMap<Voucher, CreateOrUpdateVoucherResponseDTO>();
            CreateMap<Promotion, CreateOrUpdatePromotionResponseDTO>();
            CreateMap<PromotionDetail, CreateOrUpdatePromotionDetailResponseDTO>();
            CreateMap<PromotionDetail, CreateOrUpdatePromotionDetailDTO>();
            CreateMap<CreateOrUpdatePromotionDetailDTO, PromotionDetail>();
            CreateMap<User, CreateOrUpdateUserResponseDTO>();
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : src.UserName));
            CreateMap<Employee, CreateOrUpdateEmployeeResponseDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.User.LastLogin))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.User.CreationTime))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<Order, CreateOrUpdateOrderResponse>();
            CreateMap<Customers, CreateOrUpdateCustomerResponse>();
            CreateMap<Customers, CustomerDTO>();
            CreateMap<Brand, BrandDTO>();
            CreateMap<BrandDTO, Brand>();
            CreateMap<Brand, CreateOrUpdateBrandResponseDTO>();
            CreateMap<CreateOrUpdateBrandDTO, Brand>();
            CreateMap<Season, SeasonDTO>();
            CreateMap<Season, CreateOrUpdateSeasonResponseDTO>();
            CreateMap<CreateOrUpdateSeasonDTO, Season>();
            CreateMap<Customers, CreateOrUpdateCustomerResponse>();
            CreateMap<Customers, CustomerDTO>();
            CreateMap<Material, CreateOrUpdateMaterialDTO>();
            CreateMap<Material, CreateOrUpdateMaterialResponse>();
            CreateMap<Size, SizeDTO>();
            CreateMap<Voucher, VoucherDTO>();
            CreateMap<Employee, CreateOrUpdateEmployeeDTO>();
            CreateMap<CreateOrUpdateEmployeeDTO, Employee>();
            CreateMap<CreateOrUpdateEmployeeResponseDTO, Employee>();
            CreateMap<Employee, CreateOrUpdateEmployeeResponseDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.User.LastLogin))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.User.CreationTime))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDetail, OrderDetailDTO>()
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => src.ProductDetail.Size.Value))
                .ForMember(dest => dest.ColourName, opt => opt.MapFrom(src => src.ProductDetail.Colour.Name));
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : src.UserName));
            CreateMap<ProductDetail, ProductDetailGrid>().IgnoreAllNonExisting();
            CreateMap<Image, ProductMediaUpload>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.URL))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ContentType,
                    opt => opt.MapFrom(src => src.Type == 0 ? "image/jpeg" : "video/mp4"));
            // Category mappings
            CreateMap<CategoryDTO, Category>();
            CreateMap<Category, CategoryResponseDTO>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<ProductReviewCreateOrUpdateDTO, ProductReview>();
        }
    }
}

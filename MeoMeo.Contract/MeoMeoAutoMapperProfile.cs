using AutoMapper;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract
{
    public class MeoMeoAutoMapperProfile : Profile
    {
        public MeoMeoAutoMapperProfile()
        {
            CreateMap<CreateOrUpdateProductDTO, Product>();
            CreateMap<CreateOrUpdatePromotionDTO, Promotion>();
            CreateMap<CreateOrUpdatePromotionDetailDTO, PromotionDetail>();
            CreateMap<CreateOrUpdateVoucherDTO, Voucher>();
            CreateMap<CreateOrUpdateUserDTO, User>();
            CreateMap<CreateOrUpdateEmployeeDTO, Employee>();
            CreateMap<CreateOrUpdateCustomerDTO, Customers>();
            CreateMap<CreateOrUpdateResetPasswordHistoryDTO, ResetPasswordHistory>();
            CreateMap<CreateOrUpdateBankDTO, Bank>();
            CreateMap<CreateOrUpdateCustomersBankDTO, CustomersBank>();
        }
        
    }
}

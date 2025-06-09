using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class VoucherServices : IVoucherServices
    {
        private readonly IVoucherRepository _repository;
        private readonly IMapper _mapper;

        public VoucherServices(IVoucherRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Voucher> CreateVoucherAsync(CreateOrUpdateVoucherDTO voucher)
        {
            var mappedVoucher = _mapper.Map<Voucher>(voucher);
            mappedVoucher.Id = Guid.NewGuid();
            return await _repository.AddAsync(mappedVoucher);
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            var lstAllVoucher = await _repository.GetAllAsync();

            var voucherToDelete = lstAllVoucher.FirstOrDefault(x => x.Id == id);

            if (voucherToDelete == null)
            {
                return false;
            }

            var result = await _repository.DeleteVoucherAsync(voucherToDelete);

            return result;
        }

        public async Task<List<Voucher>> GetAllVoucherAsync()
        {
            return await _repository.GetAllVoucherAsync();
        }

        public async Task<Voucher> GetVoucherByIdAsync(Guid id)
        {
            return await _repository.GetVoucherByIdAsync(id);
        }

        public async Task<Voucher> UpdateVoucherAsync(CreateOrUpdateVoucherDTO voucher)
        {
            Voucher voucherDB = new Voucher();

            voucherDB.Id = voucher.Id;
            voucherDB.Discount = voucher.Discount;
            voucherDB.Code = voucher.Code;
            voucherDB.StartDate = voucher.StartDate;
            voucherDB.EndDate = voucher.EndDate;
            voucherDB.Type = voucher.Type;
            voucherDB.MinOrder = voucher.MinOrder;
            voucherDB.MaxDiscount = voucher.MaxDiscount;
            voucherDB.MaxTotalUse = voucher.MaxTotalUse;
            voucherDB.MaxTotalUsePerCustomer = voucher.MaxTotalUsePerCustomer;


            var result = await _repository.UpdateVoucherAsync(voucherDB);
            return result;
        }
    }
}

using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
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
            return await _repository.CreateVoucherAsync(mappedVoucher);
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            var voucherToDelete = await _repository.GetVoucherByIdAsync(id);

            if (voucherToDelete == null)
            {
                return false;
            }

            await _repository.DeleteVoucherAsync(voucherToDelete.Id);
            return true;
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
            Voucher voucherCheck = new Voucher();

            voucherCheck = _mapper.Map<Voucher>(voucher);

            var result = await _repository.UpdateVoucherAsync(voucherCheck);

            return result;
        }
    }
}

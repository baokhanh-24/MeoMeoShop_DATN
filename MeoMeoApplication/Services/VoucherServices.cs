using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
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

        public async Task<CreateOrUpdateVoucherResponseDTO> GetVoucherByIdAsync(Guid id)
        {
            CreateOrUpdateVoucherResponseDTO responseDTO = new CreateOrUpdateVoucherResponseDTO();

            var check = await _repository.GetVoucherByIdAsync(id);
            if(check == null)
            {
                responseDTO.ResponseStatus = BaseStatus.Error;
                responseDTO.Message = "Không tìm thấy voucher";
                return responseDTO;
            }

            responseDTO = _mapper.Map<CreateOrUpdateVoucherResponseDTO>(check);
            responseDTO.ResponseStatus = BaseStatus.Success;
            responseDTO.Message = "";
            return responseDTO;
        }

        public async Task<CreateOrUpdateVoucherResponseDTO> UpdateVoucherAsync(CreateOrUpdateVoucherDTO voucher)
        {
            var itemVoucher = await _repository.GetVoucherByIdAsync(Guid.Parse(voucher.Id.ToString()));
            if(itemVoucher == null)
            {
                return new CreateOrUpdateVoucherResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy voucher" };
            }
            _mapper.Map(voucher, itemVoucher);

            await _repository.UpdateVoucherAsync(itemVoucher);
            return new CreateOrUpdateVoucherResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}

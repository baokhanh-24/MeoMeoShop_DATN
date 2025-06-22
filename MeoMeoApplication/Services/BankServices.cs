using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
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
    public class BankServices : IBankServices
    {
        private readonly IBankRepository _repository;
        private readonly IMapper _mapper;

        public BankServices(IBankRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Bank> CreateBankAsync(CreateOrUpdateBankDTO bank)
        {
            var mappedBank = _mapper.Map<Bank>(bank);
            mappedBank.Id = Guid.NewGuid();
            return await _repository.CreateBankAsync(mappedBank);
        }

        public async Task<bool> DeleteBankAsync(Guid id)
        {
            var bankToDelete = await _repository.GetBankByIdAsync(id);

            if (bankToDelete == null)
            {
                return false;
            }

            await _repository.DeleteBankAsync(bankToDelete.Id);
            return true;
        }

        public async Task<List<Bank>> GetListAllBankAsync()
        {
            return await _repository.GetAllBankAsync();
        }
        public async Task<PagingExtensions.PagedResult<BankDTO>> GetAllBankAsync(GetListBankRequestDTO request)
        {
            try
            {
                var query = _repository.Query();

                if (!string.IsNullOrEmpty(request.NameFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{request.NameFilter}%"));
                }

                
                query = query.OrderBy(c => c.Name);
                var filteredBanks = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);
                var dtoItems = _mapper.Map<List<BankDTO>>(filteredBanks.Items);

                return new PagingExtensions.PagedResult<BankDTO>
                {
                    TotalRecords = filteredBanks.TotalRecords,
                    PageIndex = filteredBanks.PageIndex,
                    PageSize = filteredBanks.PageSize,
                    Items = dtoItems
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<CreateOrUpdateBankResponseDTO> GetBankByIdAsync(Guid id)
        {
            CreateOrUpdateBankResponseDTO responseDTO = new CreateOrUpdateBankResponseDTO();

            var check = await _repository.GetBankByIdAsync(id);
            if (check == null)
            {
                responseDTO.ResponseStatus = BaseStatus.Error;
                responseDTO.Message = "Không tìm thấy bank";
                return responseDTO;
            }

            responseDTO = _mapper.Map<CreateOrUpdateBankResponseDTO>(check);
            responseDTO.ResponseStatus = BaseStatus.Success;
            responseDTO.Message = "";
            return responseDTO;
        }

        public async Task<CreateOrUpdateBankResponseDTO> UpdateBankAsync(CreateOrUpdateBankDTO bank)
        {
            var itemBank = await _repository.GetBankByIdAsync(Guid.Parse(bank.Id.ToString()));
            if (itemBank == null)
            {
                return new CreateOrUpdateBankResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy bank" };
            }
            _mapper.Map(bank, itemBank);

            await _repository.UpdateBankAsync(itemBank);
            return new CreateOrUpdateBankResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}

using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
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

        public async Task<List<Bank>> GetAllBankAsync()
        {
            return await _repository.GetAllBankAsync();
        }

        public async Task<Bank> GetBankByIdAsync(Guid id)
        {
            return await _repository.GetBankByIdAsync(id);
        }

        public async Task<Bank> UpdateBankAsync(CreateOrUpdateBankDTO bank)
        {
            Bank bankCheck = new Bank();

            bankCheck = _mapper.Map<Bank>(bank);

            var result = await _repository.UpdateBankAsync(bankCheck);

            return result;
        }
    }
}

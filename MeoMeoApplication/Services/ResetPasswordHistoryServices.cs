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
    public class ResetPasswordHistoryServices : IResetPasswordHistoryServices
    {
        private readonly IResetPasswordHistoryRepository _repository;
        private readonly IMapper _mapper;

        public ResetPasswordHistoryServices(IResetPasswordHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResetPasswordHistory> CreateResetPasswordHistoryAsync(CreateOrUpdateResetPasswordHistoryDTO resetPasswordHistory)
        {
            var mappedresetPasswordHistory = _mapper.Map<ResetPasswordHistory>(resetPasswordHistory);
            mappedresetPasswordHistory.Id = Guid.NewGuid();
            return await _repository.CreateResetPasswordHistoryAsync(mappedresetPasswordHistory);
        }

        public async Task<bool> DeleteResetPasswordHistoryAsync(Guid id)
        {
            var resetPasswordHistoryToDelete = await _repository.GetResetPasswordHistoryByIdAsync(id);

            if (resetPasswordHistoryToDelete == null)
            {
                return false;
            }

            await _repository.DeleteResetPasswordHistoryAsync(resetPasswordHistoryToDelete.Id);
            return true;
        }

        public async Task<List<ResetPasswordHistory>> GetAllResetPasswordHistoryAsync()
        {
            return await _repository.GetAllResetPasswordHistoryAsync();
        }

        public async Task<ResetPasswordHistory> GetResetPasswordHistoryByIdAsync(Guid id)
        {
            return await _repository.GetResetPasswordHistoryByIdAsync(id);
        }

        public async Task<ResetPasswordHistory> UpdateResetPasswordHistoryAsync(CreateOrUpdateResetPasswordHistoryDTO resetPasswordHistory)
        {
            ResetPasswordHistory resetPasswordHistoryCheck = new ResetPasswordHistory();

            resetPasswordHistoryCheck = _mapper.Map<ResetPasswordHistory>(resetPasswordHistory);

            var result = await _repository.UpdateResetPasswordHistoryAsync(resetPasswordHistoryCheck);

            return result;
        }
    }
}

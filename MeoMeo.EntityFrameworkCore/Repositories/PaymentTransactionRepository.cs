using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class PaymentTransactionRepository : BaseRepository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(MeoMeoDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PaymentTransaction?> GetByTransactionCodeAsync(string transactionCode)
        {
            return await Query()
                .Include(t => t.Order)
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);
        }

        public async Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId)
        {
            return await Query()
                .Include(t => t.Order)
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.OrderId == orderId);
        }

        public async Task<List<PaymentTransaction>> GetByCustomerIdAsync(Guid customerId)
        {
            return await Query()
                .Include(t => t.Order)
                .Include(t => t.Customer)
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<PaymentTransaction>> GetFailedTransactionsAsync()
        {
            return await Query()
                .Include(t => t.Order)
                .Include(t => t.Customer)
                .Where(t => t.Status == EPaymentStatus.Failed)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<PaymentTransaction>> GetPendingTransactionsAsync(TimeSpan timeout)
        {
            var cutoffTime = DateTime.Now.Subtract(timeout);

            return await Query()
                .Include(t => t.Order)
                .Include(t => t.Customer)
                .Where(t => t.Status == EPaymentStatus.Pending && t.CreatedAt < cutoffTime)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}

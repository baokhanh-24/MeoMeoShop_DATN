using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories
{
    public interface IPaymentTransactionRepository : IBaseRepository<PaymentTransaction>
    {
        Task<PaymentTransaction?> GetByTransactionCodeAsync(string transactionCode);
        Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId);
        Task<List<PaymentTransaction>> GetByCustomerIdAsync(Guid customerId);
        Task<List<PaymentTransaction>> GetFailedTransactionsAsync();
        Task<List<PaymentTransaction>> GetPendingTransactionsAsync(TimeSpan timeout);
    }
}

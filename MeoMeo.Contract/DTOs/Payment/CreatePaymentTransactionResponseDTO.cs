using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Payment;

public class CreatePaymentTransactionResponseDTO:BaseResponse
{
   
    public string TransactionCode { get; set; }
}
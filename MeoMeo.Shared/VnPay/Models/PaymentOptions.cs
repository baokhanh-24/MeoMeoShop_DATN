namespace MeoMeo.Shared.VnPay.Models;

public class PaymentOptions
{
    public const string SectionName = "Payment";

    public VnPayOptions Vnpay { get; set; } = new();
    public string TimeZoneId { get; set; } = "SE Asia Standard Time";

    public class VnPayOptions
    {
        public string TmnCode { get; set; } = default!;
        public string HashSecret { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
        public string Command { get; set; } = "pay";
        public string CurrCode { get; set; } = "VND";
        public string Version { get; set; } = "2.1.0";
        public string Locale { get; set; } = "vn";
        public string PaymentBackReturnUrl { get; set; } = default!;
    }
}
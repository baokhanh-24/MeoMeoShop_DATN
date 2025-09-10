namespace MeoMeo.Contract.DTOs.VietQR
{
    public class BankDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Bin { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public int TransferSupported { get; set; }
        public int LookupSupported { get; set; }
    }

    public class VietQRBankResponseDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public List<BankDTO> Data { get; set; } = new();
    }
}

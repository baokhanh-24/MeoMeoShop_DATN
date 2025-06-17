namespace MeoMeo.Contract.Commons
{
    public abstract class BaseResponse
    {
        public BaseStatus Status { get; set; }
        public string Message { get; set; }
    }
    public enum BaseStatus
    {
        Error,
        Success,
    }
}

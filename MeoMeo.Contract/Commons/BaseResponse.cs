namespace MeoMeo.Contract.Commons
{
    public abstract class BaseResponse
    {
        public BaseStatus ResponseStatus { get; set; }
        public string Message { get; set; }
    }
    public enum BaseStatus
    {
        Error,
        Success,
    }
}

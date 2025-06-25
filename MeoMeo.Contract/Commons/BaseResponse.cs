namespace MeoMeo.Contract.Commons
{
    public abstract class BaseResponse
    {
        public BaseStatus ResponseStatus { get; set; } = BaseStatus.Success;
        public string Message { get; set; } =String.Empty;
    }
    public enum BaseStatus
    {
        Error,
        Success,
    }
}

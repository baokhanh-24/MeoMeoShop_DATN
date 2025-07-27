namespace MeoMeo.Contract.Commons
{
    public class ApiResponse<T>
    {
        public BaseStatus ResponseStatus { get; set; } = BaseStatus.Success;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public bool IsSuccess => ResponseStatus == BaseStatus.Success;

        public static ApiResponse<T> Success(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                ResponseStatus = BaseStatus.Success,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> Error(string message)
        {
            return new ApiResponse<T>
            {
                ResponseStatus = BaseStatus.Error,
                Message = message
            };
        }
    }
}
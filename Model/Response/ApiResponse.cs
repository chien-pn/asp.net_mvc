namespace net_chat.Model.Response
{
    public class ApiResponse<T>(int statusCode, string message, T? data = default, object? error = null)
    {
        public int StatusCode { get; set; } = statusCode;
        public string Message { get; set; } = message;
        public T? Data { get; set; } = data;
        public object? Error { get; set; } = error;
    }
}
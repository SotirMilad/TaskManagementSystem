namespace TaskManagementSystem.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public static ApiException NotFound(string message) => new(404, message);
        public static ApiException BadRequest(string message) => new(400, message);
        public static ApiException Conflict(string message) => new(409, message);
    }
}

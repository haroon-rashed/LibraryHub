namespace LibraryHub.Api.Services
{
    public enum ServiceStatus
    {
        Success,
        NotFound,
        Conflict,
        BadRequest
    }

    public class ServiceResult<T>
    {
        public ServiceStatus Status { get; private init; }
        public T? Data { get; private init; }
        public string Message { get; private init; } = string.Empty;
        public bool IsSuccess => Status == ServiceStatus.Success;

        public static ServiceResult<T> Success(T data) =>
            new() { Status = ServiceStatus.Success, Data = data };

        public static ServiceResult<T> NotFound(string message) =>
            new() { Status = ServiceStatus.NotFound, Message = message };

        public static ServiceResult<T> Conflict(string message) =>
            new() { Status = ServiceStatus.Conflict, Message = message };

        public static ServiceResult<T> BadRequest(string message) =>
            new() { Status = ServiceStatus.BadRequest, Message = message };
    }
}
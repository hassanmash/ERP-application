namespace Application.Common
{
    /// <summary>
    /// Status enum is "HTTP-flavored" but deliberately not literally HttpStatusCode —
    /// keeps the Application layer from taking a hard dependency on
    /// Microsoft.AspNetCore.Http. The Controller is the only layer that translates
    /// this into an actual ActionResult/status code.
    /// </summary>
    public enum ServiceResultStatus
    {
        Ok,
        Created,
        BadRequest,
        Unauthorized,
        Forbidden,
        NotFound,
        Conflict
    }

    /// <summary>
    /// Standard return shape for every Service method. Controllers never inspect
    /// exceptions for expected business outcomes (validation failures, conflicts,
    /// not-found) — those are modeled explicitly here. Exceptions are reserved for
    /// truly unexpected failures (DB connection lost, etc.), which bubble up and
    /// get handled by global exception middleware separately.
    /// </summary>
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public T? Data { get; private set; }
        public ServiceResultStatus Status { get; private set; }

        private ServiceResult() { }

        public static ServiceResult<T> Success(T data, ServiceResultStatus status = ServiceResultStatus.Ok, string message = "")
            => new()
            {
                IsSuccess = true,
                Data = data,
                Status = status,
                Message = message
            };

        public static ServiceResult<T> Failure(string message, ServiceResultStatus status)
            => new()
            {
                IsSuccess = false,
                Message = message,
                Status = status
            };
    }

    public class ServiceResult
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public ServiceResultStatus Status { get; private set; }

        private ServiceResult() { }

        public static ServiceResult Success(ServiceResultStatus status = ServiceResultStatus.Ok, string message = "")
            => new()
            {
                IsSuccess = true,
                Status = status,
                Message = message
            };

        public static ServiceResult Failure(string message, ServiceResultStatus status)
            => new()
            {
                IsSuccess = false,
                Message = message,
                Status = status
            };
    }
}

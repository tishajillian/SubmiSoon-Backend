namespace SubmiSoonProject.DTOs.Common
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public PagingDto? Paging { get; set; }
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
    }

    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public ErrorDetails Error { get; set; } = null!;
    }

    public class ErrorDetails
    {
        public string Code { get; set; } = null!;
        public string Message { get; set; } = null!;
        public object? Details { get; set; }
    }
}

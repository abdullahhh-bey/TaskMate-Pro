namespace TaskMate.Core.DTO.ErrorDTO
{
    public class ErrorMessage
    {
        public string Message { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}

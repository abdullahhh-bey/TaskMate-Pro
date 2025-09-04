namespace TaskMate.Core.DTO.AuthDTO
{
    public class ConfirmEmailDTO
    {
        public required string Email { get; set; } = string.Empty;
        public required string ConfirmEmailToken { get; set; } = string.Empty;
    }
}

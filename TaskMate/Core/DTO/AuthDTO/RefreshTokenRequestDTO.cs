namespace TaskMate.Core.DTO.AuthDTO
{
    public class RefreshTokenRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty; 
    }
}

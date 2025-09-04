namespace TaskMate.Core.DTO.AuthDTO
{
    public class ResponseTokensDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}

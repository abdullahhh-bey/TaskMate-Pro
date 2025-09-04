using System.ComponentModel.DataAnnotations;

namespace TaskMate.Core.DTO.AuthDTO
{
    public class RegisterDTO
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}

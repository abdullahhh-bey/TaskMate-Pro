using System.ComponentModel.DataAnnotations;

namespace TaskMate.Core.DTO.AuthDTO
{
    public class ForgetPasswordDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}

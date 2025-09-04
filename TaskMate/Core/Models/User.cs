using Microsoft.AspNetCore.Identity;

namespace TaskMate.Core.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

    }
}

using Microsoft.AspNetCore.Identity;
using TaskMate.Core.Models;

namespace TaskMate.Infrastructure.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int id);
        Task<IdentityResult> AddUser(User user, string password);
        Task<string> GenerateEmailConfirmToken(User user);
        Task AddRole(User user, string role);
        Task<IdentityResult> ConfirmEmail(User user, string token);
        Task<string> PasswordResetToken(User user);
        Task<bool> ResetPassword(User user, string token, string newPassword);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IList<string>> GetRolesAsync(User user);
        Task<bool> UpdateUser(User user);

    }
}

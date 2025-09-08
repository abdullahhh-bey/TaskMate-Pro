using Microsoft.AspNetCore.Identity;
using TaskMate.Core.Models;
using TaskMate.Infrastructure.Data;

namespace TaskMate.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(UserManager<User> userManager, ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }


        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task AddRole(User user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> AddUser(User user, string pass)
        {
            _logger.LogInformation("User {Name} Created in Database with {Email} at {Time}",user.FullName, user.Email, DateTime.UtcNow);
            return await _userManager.CreateAsync(user, pass);
        }


        public async Task<IdentityResult> ConfirmEmail(User user, string token)
        {
            _logger.LogInformation("User {Name} Confirmed {Email} at {Time}", user.FullName, user.Email, DateTime.UtcNow);
            return await _userManager.ConfirmEmailAsync(user, token);
         
        }


        public async Task<bool> UpdateUser(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<string> GenerateEmailConfirmToken(User user)
        {
            _logger.LogInformation("User {Name} Created {Email} Confirmation Token at {Time}", user.FullName, user.Email, DateTime.UtcNow);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }


        public async Task<User> GetUserByEmail(string email)
        {
            _logger.LogInformation("Accessing User in Database with {Email} at {Time}", email, DateTime.UtcNow);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            return user;
        }


        public async Task<User> GetUserById(int id)
        {
            _logger.LogInformation("Accessing User in Database with {ID} at {Time}", id, DateTime.UtcNow);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;

            return user;
        }


        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }


        public async Task<string> PasswordResetToken(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<bool> ResetPassword(User user, string token, string newPassword)
        {
            var check = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if(!check.Succeeded)
                return false;

            return true;
        }



        public async Task<bool> ChangePassword(User user, string currentPassword, string newPassword)
        {
            var check = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!check.Succeeded)
                return false;

            return true;
        }


    }
}

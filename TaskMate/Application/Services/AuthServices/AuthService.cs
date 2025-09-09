using TaskMate.Application.Services.EmailServices;
using TaskMate.Core.DTO.AuthDTO;
using TaskMate.Core.Models;
using TaskMate.Infrastructure.AuthService;
using TaskMate.Infrastructure.Repository;

namespace TaskMate.Application.Services.AuthServices
{
    public class AuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public AuthService(ILogger<AuthService> logger, JwtService jwtService, IUserRepository userRepository, EmailService emailService)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<bool> RegisterService(RegisterDTO dto)
        {
            _logger.LogInformation("Registering User {Name} with {Email} in {Role} at {Time}", dto.FullName, dto.Email, dto.Role, DateTime.UtcNow);

            var check = await _userRepository.GetUserByEmail(dto.Email);
            if (check != null)
                return false;

            var user = new User
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userRepository.AddUser(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadHttpRequestException(errors);
            }



            await _userRepository.AddRole(user, dto.Role);


            var EmailConfirmtoken = await _userRepository.GenerateEmailConfirmToken(user);
            var subject = $"Email Confirmation from TaskMate";
            var message = $"To Complete your Registration: Use this token: {EmailConfirmtoken}\nThank You from TEAM TASKMATE PRO.";

            _logger.LogInformation("Sent Confirmation Email to {Email} at {Time}", dto.Email, DateTime.UtcNow);

            await _emailService.SendEmailAsync(dto.Email, subject, message);
            return true;
        }



        public async Task<bool> EmailConfirmationService(ConfirmEmailDTO dto)
        {
            var user = await _userRepository.GetUserByEmail(dto.Email);
            if (user == null)
                return false;

            var checkToken = await _userRepository.ConfirmEmail(user, dto.ConfirmEmailToken);
            if (!checkToken.Succeeded)
                throw new UnauthorizedAccessException("Invalid Token. Failed Attempt!");

            user.EmailConfirmed = true;
            await _userRepository.UpdateUser(user);

            return true;
        }


        public async Task<ResponseTokensDTO> LoginService(LoginDTO dto)
        {
            var user = await _userRepository.GetUserByEmail(dto.Email);
            if (user == null)
                throw new KeyNotFoundException("Email not registered!");

            if (!user.EmailConfirmed)
                throw new BadHttpRequestException("Cannot Login right now! Your email not confirme yet.");

            var checkPassword = await _userRepository.CheckPasswordAsync(user, dto.Password);
            if (!checkPassword)
            {
                _logger.LogWarning("Failed Attempt to login {Email} at {Time}", dto.Email, DateTime.UtcNow);
                throw new BadHttpRequestException("Invalid Password!");
            }

            var accessToken = await _jwtService.GenerateToken(user);
            var refreshToken = await _jwtService.GenerateRefreshToken(user);

            _logger.LogInformation("User Login Confirmed for {Email} at {Time}", dto.Email, DateTime.UtcNow);

            return new ResponseTokensDTO {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }


        public async Task<bool> ChangePassword(ChangePasswordDTO dto)
        {
            var user = await _userRepository.GetUserByEmail(dto.Email);
            if (user == null)
                throw new KeyNotFoundException("Email not registered!");

            var checkpassword = await _userRepository.ChangePassword(user, dto.CurrentPassword, dto.NewPassword);
            if (!checkpassword)
            {
                _logger.LogWarning("Failed Attempt to change password for {Email} at {Time}", dto.Email, DateTime.UtcNow);
                throw new BadHttpRequestException("Invalid Password!\nCheck Password");
            }

            _logger.LogInformation("Password Changed Successfully for {Email} at {Time}", dto.Email, DateTime.UtcNow);
            return true;
        }

        public async Task<bool> ForgetPasswordService(ForgetPasswordDTO dto)
        {
            var user = await _userRepository.GetUserByEmail(dto.Email);
            if (user == null)
                throw new KeyNotFoundException("Email not registered!");

            var token = await _userRepository.PasswordResetToken(user);
            var subject = $"Reset Password Token";
            var message = $"Reset your Password for {dto.Email}\nToken: {token}\n From Team TaskMate";

            _logger.LogInformation("Sent Reset Password Email for {Email} at {Time}", dto.Email , DateTime.UtcNow);
            await _emailService.SendEmailAsync( dto.Email , subject , message );

            return true;
        }



        public async Task<bool> ResetPasswordService( ResetPasswordDTO dto)
        {
            var user = await _userRepository.GetUserByEmail(dto.Email);
            if (user == null)
                throw new KeyNotFoundException("Email not registered");

            var resetToken = await _userRepository.ResetPassword(user, dto.Token, dto.NewPassword);
            if(!resetToken)
            {
                _logger.LogWarning("Check token or password! Attempt Failed for {Email} with {Token}!", dto.Email, dto.Token);
                return false;
            }

            return true;
        }

        //Refresh Tokens Testing completed

        public async Task<ResponseTokensDTO> ValidateCreateAccessTokenService(RefreshTokenRequestDTO dto)
        {
            return await _jwtService.ValidateGenerateTokens(dto);
        }




    }
}

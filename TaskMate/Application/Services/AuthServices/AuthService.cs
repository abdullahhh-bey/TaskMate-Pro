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
                throw new BadHttpRequestException("Invalid Password!");


            await _userRepository.AddRole(user, dto.Role);


            var EmailConfirmtoken = await _userRepository.GenerateEmailConfirmToken(user);
            var subject = $"Email Confirmation from TaskMate";
            var message = $"To Complete your Registration: Use this token: {EmailConfirmtoken}\nThank You from TEAM TASKMATE PRO.";

            _logger.LogInformation("Sent Confirmation Email to {Email} at {Time}", dto.Email, DateTime.UtcNow);

            await _emailService.SendEmailAsync(dto.Email, subject , message);
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


    }
}

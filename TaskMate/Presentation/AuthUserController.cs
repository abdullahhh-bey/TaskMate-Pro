using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskMate.Application.Services.AuthServices;
using TaskMate.Core.DTO.AuthDTO;

namespace TaskMate.Presentation
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private readonly ILogger<AuthUserController> _logger;
        private readonly AuthService _service;

        public AuthUserController(ILogger<AuthUserController> logger, AuthService service)
        {
            _logger = logger;
            _service = service;     
        }



        [HttpPost("register")]
        public async Task<IActionResult> RegisterAPI(RegisterDTO dto)
        {

            if (!ModelState.IsValid)
                return BadRequest("Incomplete Information");

            var check = await _service.RegisterService(dto);
            if (!check)
            {
                _logger.LogWarning("Email {Email} already in use, {Time}", dto.Email, DateTime.Now);
                throw new BadHttpRequestException("Email already in use.");
            }

            return Ok("An Email has been sent to your gmail.\nPlease confirm it for completing registration!");
        }


        [HttpPost("confirm-email")]
        public async Task<IActionResult> EmailConfirmationAPI(ConfirmEmailDTO dto)
        {

            if (!ModelState.IsValid)
                return BadRequest("Incomplete Information");

            var check = await _service.EmailConfirmationService(dto);
            if (!check)
                throw new KeyNotFoundException("Email not found.\nInvalid Email!");

            _logger.LogInformation("Email Confirmation Completed for {Email} at {Time}", dto.Email, DateTime.UtcNow);
            return Ok("Email Confirmed Successfully!.\nNow, you can login.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginAPI(LoginDTO dto)
        {

            if (!ModelState.IsValid)
                return BadRequest("Incomplete Information");

            var token = await _service.LoginService(dto);

            _logger.LogInformation("User with {Email} logged in at {Time}", dto.Email, DateTime.UtcNow);
            return Ok(new { Message = "Login Successfully!" ,Token = token.AccessToken , RefreshToken = token.RefreshToken });
        }




        [HttpGet("validate-token")]
        [Authorize]
        public async Task<IActionResult> SampleAPi()
        {
            return Ok("Your login api is working");
        }


    }
}

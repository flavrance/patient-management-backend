using System;
using System.Threading.Tasks;
using FinX.Application.DTOs.Auth;
using FinX.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthService authService,
            IValidator<LoginDto> loginValidator,
            ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var validationResult = await _loginValidator.ValidateAsync(loginDto);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var response = _authService.GenerateToken(loginDto);
                if (response == null)
                    return Unauthorized("Credenciais inválidas");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no login");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 
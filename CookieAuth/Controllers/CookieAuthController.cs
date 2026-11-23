using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BaseCRUD.CookieAuth.Repos;
using BaseCRUD.CookieAuth.Services;
using BaseCRUD.CookieAuth.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BaseCRUD.CookieAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CookieAuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;

        public CookieAuthController(IUserRepository userRepository, PasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.DisplayName))
            {
                return Conflict("All Fields are required.");
            }

            var email = request.Email.Trim();
            var normalizedEmail = email.ToLowerInvariant();

            if (await _userRepository.EmailExistsAsync(normalizedEmail))
            {
                return Conflict("User with this email already exists.");
            }

            var user = new Users
            {
                Email = email,
                NormalizedEmail = normalizedEmail,
                DisplayName = request.DisplayName.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = _passwordService.HashPassword(request.Password)
            };
            var createdUserID = await _userRepository.CreateAsync(user);
            return Created($"auth/{createdUserID}", new { 
                UserId = createdUserID, user.Email, user.DisplayName });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if(string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Email and Password are required.");
            }
            var normalizedEmail = loginRequest.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            var isPasswordValid = _passwordService.VerifyPassword(loginRequest.Password, user.PasswordHash);

            if (user == null || !user.IsActive || !isPasswordValid)
                return Unauthorized("Invalid email or password.");

            List<Claim> claims =
                [ new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.DisplayName)];
              
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            return Ok(new { 
                Message = "Login successful.",
                user.UserId,
                user.Email,
                user.DisplayName
            });
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("Cookie-Profile")]
        public IActionResult CookieProfile()
        {
            return Ok(new { 
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                DisplayName = User.FindFirst(ClaimTypes.Name)?.Value
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }
    }
}

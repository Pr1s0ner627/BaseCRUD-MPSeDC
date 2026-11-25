using Microsoft.AspNetCore.Mvc;
using BaseCRUD.CookieAuth.Repos;
using BaseCRUD.CookieAuth.Models;
using BaseCRUD.JWTAuth.Services;
using BaseCRUD.CookieAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace BaseCRUD.JWTAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize, AllowAnonymous]
    public class JWTAuthController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;
        private readonly RefreshTokenService _refreshTokenService;

        public JWTAuthController(IUserRepository userRepository, PasswordService passwordService, JWTService jwtService, RefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
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

        [HttpPost("login-jwt")]
        public async Task<IActionResult> LoginJWT(LoginRequest loginRequest)
        {
            if(string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return Unauthorized("Email and Password are required.");
            }
            var normalizedEmail = loginRequest.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (user == null || !user.IsActive)
                return Unauthorized("Invalid email or password.");

            var isPasswordValid = _passwordService.VerifyPassword(loginRequest.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized("Invalid email or password.");
            
            var token = _jwtService.GenerateToken(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.UserId);
            
            //Response.Headers.Add("AccessToken", token);
            return Ok(new
            {
                Message = "Login successful.",
                AccessToken = token,
                RefreshToken = refreshToken
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            return Ok(new
                {
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Email = User.FindFirstValue(ClaimTypes.Email),
                    DisplayName = User.FindFirstValue(ClaimTypes.Name)
                });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Headers["AccessToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized("Refresh Token is Missing.");

            var storedToken = await _refreshTokenService.ValidateRefreshTokenAsync(refreshToken);
            var newRefreshToken = await _refreshTokenService.RotateRefreshTokenAsync(refreshToken);
            if(newRefreshToken == null && storedToken == null)
                return Unauthorized("Invalid or Expired Token.");

            var user = await _userRepository.GetbyIdAsync(storedToken.UserId);

            if (user == null || !user.IsActive)
                return Unauthorized("User Account is unavailable");

            var newAccessToken = _jwtService.GenerateToken(user);
            Response.Headers.Add("RefreshToken", newAccessToken);
            
            return Ok(new {Message = "Token Refreshed Successfully.",
            NewToken = newAccessToken});
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(JwtBearerDefaults.AuthenticationScheme);
            return Ok(new { Message = "Logged Out"});
        }
    }
}

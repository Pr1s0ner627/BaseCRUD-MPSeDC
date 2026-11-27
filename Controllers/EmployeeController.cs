using BaseCRUD.CookieAuth.Models;
using BaseCRUD.CookieAuth.Repos;
using BaseCRUD.CookieAuth.Services;
using BaseCRUD.Databases;
using BaseCRUD.JWTAuth.Controllers;
using BaseCRUD.JWTAuth.Services;
using BaseCRUD.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseCRUD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "CookieOrJWT")]

    public class EmployeeController : ControllerBase
    {
        private readonly JWTService _jWTService;
        private readonly PasswordService _passwordService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;

        public EmployeeController(PasswordService passwordService, IEmployeeRepository employeeRepository, JWTService jWTService, IUserRepository userRepository)
        {
            _passwordService = passwordService;
            _employeeRepository = employeeRepository;
            _jWTService = jWTService;
            _userRepository = userRepository;
        }

        
        [AllowAnonymous]
        [HttpPost("AuthEntry")]
        public async Task<IActionResult> Enter(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                return BadRequest("Email and Password are required.");

            var normalizedEmail = loginRequest.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (user == null || !user.IsActive)
                return Unauthorized("Invalid email or password.");

            var isPasswordValid = _passwordService.VerifyPassword(loginRequest.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized("Invalid email or password.");

            var now = DateTimeOffset.UtcNow;
            List<Claim> claims = [
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.DisplayName),
                new(ClaimTypes.Email, user.NormalizedEmail),
                new("issued_Utc",now.ToUnixTimeSeconds().ToString())];

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var AccessToken = _jWTService.GenerateToken(user);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                IssuedUtc = now
            });

            return Ok(new
            {
                Message = "Login successful.",
                Token = AccessToken,
                user.UserId,
                user.Email,
                user.DisplayName
                
            });
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return Ok(employees);
        }

        //[HttpGet("GetbyId/{id}")]
        //public async Task<ActionResult<Employee>> GetEmployee(int id)
        //{
        //    var entry = await _employeeRepository.GetbyIdAsync(id);

        //    if (entry == null)
        //        return NotFound();

        //    return Ok(entry);
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult> RemoveEmployee(int id)
        //{
        //    var caseToRemove = await databaseContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
        //    if (caseToRemove == null)
        //        return NotFound();

        //    databaseContext.Employees.Remove(caseToRemove);
        //    await databaseContext.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpPost("AddUser")]
        public async Task<ActionResult<Employee>> AddEmployee(Employee newEmployee)
        {
            int newEmployeeID = await _employeeRepository.CreateAsync(newEmployee);
            newEmployee.Id = newEmployeeID;
            return CreatedAtAction(nameof(GetAll), new { id = newEmployee.Id }, newEmployee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmployee([FromRoute] int id, [FromBody] Employee updatedEmployee)
        {
            bool employeeToUpdate = await _employeeRepository.UpdateAsync(id, updatedEmployee);
            if (employeeToUpdate == null)
                return NotFound();

            return NoContent();
        }
    }
}

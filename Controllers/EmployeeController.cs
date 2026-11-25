using BaseCRUD.CookieAuth.Models;
using BaseCRUD.CookieAuth.Repos;
using BaseCRUD.CookieAuth.Services;
using BaseCRUD.Databases;
using BaseCRUD.JWTAuth.Controllers;
using BaseCRUD.JWTAuth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        private readonly IUserRepository _userRepository;

        public EmployeeController(PasswordService passwordService, IUserRepository userRepository, JWTService jWTService)
        {
            _passwordService = passwordService;
            _userRepository = userRepository;
            _jWTService = jWTService; 
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

            List<Claim> claims = [
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.DisplayName),
                new(ClaimTypes.Email, user.NormalizedEmail)];

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var AccessToken = _jWTService.GenerateToken(user);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
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


        //[HttpGet]
        //public async Task<ActionResult> GetAll()
        //{
        //    var employees = await databaseContext.Employees.ToListAsync();
        //    return Ok(employees);
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<Employee>> GetEmployee(int id)
        //{
        //    var entry = await databaseContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

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

        //[HttpPost]
        //public async Task<ActionResult<Employee>> AddEmployee(Employee newEmployee)
        //{
        //    databaseContext.Employees.Add(newEmployee);
        //    databaseContext.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetEmployee), new { id = newEmployee.Id }, newEmployee);
        //}

        //[HttpPut("{id}")]
        //public async Task<ActionResult> UpdateEmployee([FromRoute] int id, [FromBody] Employee updatedEmployee)
        //{
        //    var employeeToUpdate = await databaseContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
        //    if (employeeToUpdate == null)
        //        return NotFound();

        //    employeeToUpdate.Name = updatedEmployee.Name;
        //    employeeToUpdate.Email = updatedEmployee.Email;
        //    employeeToUpdate.Designation = updatedEmployee.Designation;
        //    employeeToUpdate.Salary = updatedEmployee.Salary;
        //    employeeToUpdate.DepartmentId = updatedEmployee.DepartmentId;
        //    employeeToUpdate.ManagerId = updatedEmployee.ManagerId;
        //    employeeToUpdate.IsActive = updatedEmployee.IsActive;

        //    await databaseContext.SaveChangesAsync();
        //    return Ok();
        //}
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BaseCRUD.Databases;

namespace BaseCRUD.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class EmployeeController : ControllerBase
    {
        private readonly DatabaseContext databaseContext;

        public EmployeeController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var employees = await databaseContext.Employees.ToListAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var entry = await databaseContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (entry == null)
                return NotFound();

            return Ok(entry);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveEmployee(int id)
        {
            var caseToRemove = await databaseContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (caseToRemove == null)
                return NotFound();

            databaseContext.Employees.Remove(caseToRemove);
            await databaseContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee(Employee newEmployee)
        {
            databaseContext.Employees.Add(newEmployee);
            databaseContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = newEmployee.Id }, newEmployee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmployee([FromRoute] int id, [FromBody] Employee updatedEmployee)
        {
            var employeeToUpdate = await databaseContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employeeToUpdate == null)
                return NotFound();

            employeeToUpdate.Name = updatedEmployee.Name;
            employeeToUpdate.Email = updatedEmployee.Email;
            employeeToUpdate.Designation = updatedEmployee.Designation;
            employeeToUpdate.Salary = updatedEmployee.Salary;
            employeeToUpdate.DepartmentId = updatedEmployee.DepartmentId;
            employeeToUpdate.ManagerId = updatedEmployee.ManagerId;
            employeeToUpdate.IsActive = updatedEmployee.IsActive;

            await databaseContext.SaveChangesAsync();
            return Ok();
        }
    }
}

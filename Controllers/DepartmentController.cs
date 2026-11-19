using BaseCRUD.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BaseCRUD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly DatabaseContext databaseContext;
        public DepartmentController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var department = await databaseContext.Departments.ToListAsync();
            return Ok(department);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveDept(int id)
        {
            var deptID = await databaseContext.Departments.FirstOrDefaultAsync(x => x.Id == id);

            if (deptID == null)
                return NotFound();

            else
                databaseContext.Departments.Remove(deptID);

            await databaseContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> AddDept(Department newDepartment)
        {
            databaseContext.Departments.Add(newDepartment);
            await databaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = newDepartment.Id }, newDepartment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDept([FromRoute] int id, [FromBody] Department UpdateDepartment)
        {
            var department = await databaseContext.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (department == null)
                return NotFound();

            department.Name = UpdateDepartment.Name;
            
            await databaseContext.SaveChangesAsync();
            return Ok(department);
        }
    }
}

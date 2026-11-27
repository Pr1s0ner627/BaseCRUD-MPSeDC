using BaseCRUD.CookieAuth.Models;
using BaseCRUD.Databases;

namespace BaseCRUD.Repos
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync();
        Task<int> CreateAsync(Employee employee);
        Task<bool> UpdateAsync(int employeeId, Employee employee);
    }
}

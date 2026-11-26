using BaseCRUD.Databases;

namespace BaseCRUD.Repos
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync();
    }
}

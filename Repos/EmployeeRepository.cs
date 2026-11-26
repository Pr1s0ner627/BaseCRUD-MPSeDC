using BaseCRUD.CookieAuth.Data;
using BaseCRUD.CookieAuth.Models;
using BaseCRUD.CookieAuth.Repos;
using BaseCRUD.Databases;
using Microsoft.Data.SqlClient;

namespace BaseCRUD.Repos
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly SQLConnectionFactory _connectionFactory;

        public EmployeeRepository(SQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            const string query = @"
                SELECT
                    EmployeeID,
                    EmployeeName,
                    EmployeeEmail,
                    Designation,
                    Salary,
                    ManagerID,
                    IsActive,
                    DepartmentID
                FROM Employees;";

            var employees = new List<Employee>();

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            int idOrdinal = reader.GetOrdinal("EmployeeID");
            int nameOrdinal = reader.GetOrdinal("EmployeeName");
            int emailOrdinal = reader.GetOrdinal("EmployeeEmail");
            int designationOrdinal = reader.GetOrdinal("Designation");
            int salaryOrdinal = reader.GetOrdinal("Salary");
            int managerIdOrdinal = reader.GetOrdinal("ManagerId");
            int isActiveOrdinal = reader.GetOrdinal("IsActive");
            int departmentIdOrdinal = reader.GetOrdinal("DepartmentId");

            while (await reader.ReadAsync())
            {
                employees.Add(new Employee
                {
                    Id = reader.GetInt32(idOrdinal),
                    Name = reader.GetString(nameOrdinal),
                    Email = reader.GetString(emailOrdinal),
                    Designation = reader.GetString(designationOrdinal),
                    Salary = reader.GetDecimal(salaryOrdinal),

                    ManagerId = reader.IsDBNull(managerIdOrdinal)
                        ? null
                        : reader.GetInt32(managerIdOrdinal),

                    IsActive = reader.GetBoolean(isActiveOrdinal),
                    DepartmentId = reader.GetInt32(departmentIdOrdinal)
                });
            }

            return employees;
        }
    }
}

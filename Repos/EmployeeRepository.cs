using BaseCRUD.CookieAuth.Data;
using BaseCRUD.CookieAuth.Models;
using BaseCRUD.CookieAuth.Repos;
using BaseCRUD.Databases;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public async Task<int> CreateAsync(Employee employee)
        {
            const string query = @"INSERT INTO Employees (
                    EmployeeName,
                    EmployeeEmail,
                    Designation,
                    Salary,
                    ManagerID,
                    IsActive,
                    DepartmentID) OUTPUT INSERTED.EmployeeID VALUES (@EmployeeName,
                    @EmployeeEmail,
                    @Designation,
                    @Salary,
                    @ManagerID,
                    @IsActive,
                    @DepartmentID);";

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmployeeName", employee.Name);
            command.Parameters.AddWithValue("@EmployeeEmail", employee.Email);
            command.Parameters.AddWithValue("@Designation", employee.Designation);
            command.Parameters.AddWithValue("@Salary", employee.Salary);
            command.Parameters.AddWithValue("@ManagerID", employee.ManagerId);
            command.Parameters.AddWithValue("@IsActive", employee.IsActive);
            command.Parameters.AddWithValue("@DepartmentID", employee.DepartmentId);

            object? result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(int employeeId, Employee employee)
        {
            const string query = @"
                    UPDATE Employees
                    SET
                    EmployeeName = @EmployeeName,
                    EmployeeEmail = @EmployeeEmail,
                    Designation = @Designation,
                    Salary = @Salary,
                    ManagerID = @ManagerID,
                    IsActive = @IsActive,
                    DepartmentID = @DepartmentID
                    WHERE EmployeeID = @EmployeeId;";
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmployeeID", employeeId);
            command.Parameters.AddWithValue("@EmployeeName", employee.Name);
            command.Parameters.AddWithValue("@EmployeeEmail", employee.Email);
            command.Parameters.AddWithValue("@Designation", employee.Designation);
            command.Parameters.AddWithValue("@Salary", employee.Salary);
            command.Parameters.AddWithValue("@ManagerID", (object?)employee.ManagerId??DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", employee.IsActive);
            command.Parameters.AddWithValue("@DepartmentID", employee.DepartmentId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

    }
}

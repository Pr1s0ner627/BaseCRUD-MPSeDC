using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseCRUD.Databases
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EmployeeID")]
        public int Id { get; set; }

        [Required]
        [Column("EmployeeName")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("EmployeeEmail")]
        public string Email { get; set; } = string.Empty;

        [Column("Designation")]
        public string Designation { get; set; } = string.Empty;

        [Column("ManagerID")]
        public int? ManagerId { get; set; }

        [Column("Salary")]
        public decimal? Salary { get; set; }
        
        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("DepartmentID")]
        public int DepartmentId { get; set; } 
        
        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; } = null!;
    }
}

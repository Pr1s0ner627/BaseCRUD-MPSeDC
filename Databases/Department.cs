using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseCRUD.Databases
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DepartmentID")]
        public int Id { get; set; }

        [Required]
        [Column("DepartmentName")]
        public string Name { get; set; } = string.Empty;
    }
}

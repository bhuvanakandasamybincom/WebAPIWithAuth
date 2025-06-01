using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BoardCasterWebAPI.DbContexts
{
    //public class EmployeeDbContext : DbContext
    //{
    //   // public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
    //   //: base(options)
    //   // {
    //   // }
    //    [Required]
    //    public DbSet<Employee>? Employee { get; set; }
    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseSqlServer("Server=DINESHKUMAR\\DEVSERVER;Database=webapi;User Id=sa;Password=Password#1;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=true;");
    //    }

    //}
    public class Employee
    {
        public Employee()
        {
            this.Name = name;
            this.Address = address;
        }
        private string name = string.Empty;  //Field/variable name
        private string address = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        //[ForeignKey("Level")]
        public int LevelId { get; set; }

        [Required]
        [StringLength(150)]
        [MaxLength(150)]
        public required string Name  //Property
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        [Required]
        [StringLength(255)]
        [MaxLength(255)]
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI.Entities;

namespace BoardCasterWebAPI.DbContexts
{
    /// <summary>
    /// Provides a database context for the application using Entity Framework Core and Identity Framework.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext class with the specified DbContext options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
            // The base constructor handles initializing the DbContext with the provided options.
        }


        [Required]
        public DbSet<Employee>? Employee { get; set; }
    }
}

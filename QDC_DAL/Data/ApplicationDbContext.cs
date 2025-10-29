using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QDC_DAL.Models;


namespace QDC_DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}

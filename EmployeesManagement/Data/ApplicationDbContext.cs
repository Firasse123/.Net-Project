using EmployeesManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<JobOpening> JobOpenings { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<SalaryHistory> SalaryHistories { get; set; }
    }
}

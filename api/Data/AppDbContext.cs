using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data.Configurations;
using HRMS.api.entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<RequestSubType> RequestSubTypes { get; set; }
        public DbSet<EmployeeTransaction> EmployeeTransactions { get; set; }
        public DbSet<SalaryType> SalaryTypes { get; set; }
        public DbSet<SalaryComponent> SalaryComponents { get; set; }
        public DbSet<EmployeeBaseSalary> EmployeeBaseSalaries { get; set; }
        public DbSet<SalaryHistory> SalaryHistories { get; set; }
        public DbSet<Employee> Employees {get;set;}
        public DbSet<Otp> Otps {get;set;}
        public DbSet<Schedule> Schedules {get;set;}
        public DbSet<RequestBalance> RequestBalances {get;set;}


        public AppDbContext()
        {
        }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // Now configure your entities
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            builder.Entity<User>().ToTable("User");
            builder.Entity<IdentityRole<int>>().ToTable("Role");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRole");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaim");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogin");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserToken");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaim");


            List<IdentityRole<int>> roles = new List<IdentityRole<int>>
            {
                new IdentityRole<int>
                {
                    Id = 1,
                    Name = "HR",
                    NormalizedName = "HR",
                    ConcurrencyStamp = "HRConcurrencyStamp123"
                },
                new IdentityRole<int>
                {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "UserConcurrencyStamp456"
                },
                new IdentityRole<int>
                {
                    Id = 3,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "AdminConcurrencyStamp456"
                },
            };
            builder.Entity<IdentityRole<int>>().HasData(roles);

            



        }


    }
}
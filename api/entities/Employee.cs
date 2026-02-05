using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HRMS.api.entities
{
    public class User : IdentityUser<int>
    {
        public string? ProfilePicture { get; set; }

        [Required]
        public string FullName { get; set; }  
        
        public bool IsActive { get; set; } = true;

        
        public Employee? Employee { get; set; }

        public ICollection<Otp> Otps {get;set;}

    }

    public class Employee
    {
        
        public int UserId { get; set; }
        public User? User { get; set; }
        public int? ManagerId { get; set; }
        public int? DepartmentId { get; set; }

        public string? SSN { get; set; }
        public string? WorkEmail { get; set; }
        public string? WorkPhone { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public char? Gender { get; set; }
        public DateTimeOffset? LastWorkingDate { get; set; }
        public int? PositionId { get; set; }

        public Employee? Manager { get; set; }
        public Department? Department { get; set; } = null!;
        public Position? Position { get; set; }

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();


        // public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Request>? Requests { get; set; } = new List<Request>();
        public EmployeeBaseSalary? BaseSalary { get; set; }
        public ICollection<SalaryComponent>? SalaryComponents { get; set; } = new List<SalaryComponent>();
        public ICollection<SalaryHistory>? SalaryHistories { get;set;} = new List<SalaryHistory>();
        // public ICollection<Overtime> Overtimes { get; set; } = new List<Overtime>();
        // public ICollection<Shortage> Shortages { get; set; } = new List<Shortage>();
        public ICollection<EmployeeTransaction>? Transactions { get; set; } = new List<EmployeeTransaction>();

        
    }
}
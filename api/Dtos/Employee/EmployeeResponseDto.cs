using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Employee
{
    public class EmployeeResponseDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        
        public string? SSN { get; set; }
        public string? WorkEmail { get; set; }
        public string? WorkPhone { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public char? Gender { get; set; }
        public DateTimeOffset? LastWorkingDate { get; set; }
        
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        
        public int? PositionId { get; set; }
        public string? PositionTitle { get; set; }
    }
}
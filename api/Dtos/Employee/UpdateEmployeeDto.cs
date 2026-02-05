using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Employee
{
    public class UpdateEmployeeDto
    {
        public int? ManagerId { get; set; }
        public int? DepartmentId { get; set; }
        public string? WorkEmail { get; set; }
        public string? WorkPhone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? SSN {get;set;}
        public char Gender {get;set;}
        public DateTimeOffset? LastWorkingDate { get; set; }
        public int? PositionId { get; set; }
        
    }
}
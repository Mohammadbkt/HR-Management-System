using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Employee;

namespace HRMS.api.Dtos.Department
{
    public class DepartmentDetailDto
    {
         public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int EmployeeCount {get;set;}
        public List<EmployeeResponseDto> Employees { get; set; } = new();
    }
}
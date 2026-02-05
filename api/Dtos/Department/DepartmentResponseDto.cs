using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Department
{
    public class DepartmentResponseDto
    {
    
        public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
    
    }
}
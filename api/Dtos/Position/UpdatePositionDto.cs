using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Position
{
    public class UpdatePositionDto
    {
        public string? Title { get; set; } = string.Empty;
        
        public decimal? DefaultBaseSalary { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }    
    }
}
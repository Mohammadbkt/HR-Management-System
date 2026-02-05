using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.EmployeeBaseSalary
{
    public class UpdateEmployeeBaseSalaryDto
    {
        
        public decimal BaseSalary { get; set; }
        // public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
    }
}
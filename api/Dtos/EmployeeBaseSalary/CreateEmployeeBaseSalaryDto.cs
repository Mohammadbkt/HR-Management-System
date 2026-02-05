using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.EmployeeBaseSalary
{
    public class CreateEmployeeBaseSalaryDto
    {
        public decimal BaseSalary { get; set; }
        public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly? EffectiveTo { get; set; }
    }
}
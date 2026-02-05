using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class SalaryComponent
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int SalaryTypeId { get; set; }
        public decimal Amount { get; set; }

        public Employee Employee { get; set; } = null!;
        public SalaryType SalaryType { get; set; } = null!;

    }
}
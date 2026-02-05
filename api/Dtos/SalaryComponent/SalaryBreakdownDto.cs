using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.SalaryComponent
{
    public class SalaryBreakdownDto
    {
        
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = String.Empty;

        public IEnumerable<ResponseDto> Allowances { get; set; } = new List<ResponseDto>();
        public IEnumerable<ResponseDto> Deductions { get; set; } = new List<ResponseDto>();
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }
    }
}
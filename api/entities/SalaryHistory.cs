using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class SalaryHistory
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }


        public Employee Employee { get; set; } = null!;

        public void CalculateValues()
        {
            GrossSalary = BaseSalary + TotalAllowances;
            NetSalary = GrossSalary - TotalDeductions;
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class SalaryType
    {
        public int Id { get; set; }
        public SalaryCategory Category { get; set; }
        public string SubType { get; set; } = string.Empty;



    }

    public enum SalaryCategory
{
    Allowance,
    Deduction
}
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.SalaryComponent
{
    public class CreateSalaryComponentDto
    {[  
        Required]
        public int EmployeeId { get; set; }
        
        [Required]
        public int SalaryTypeId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
    }
}
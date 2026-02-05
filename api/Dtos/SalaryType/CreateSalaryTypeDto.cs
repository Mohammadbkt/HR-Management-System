using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;

namespace HRMS.api.Dtos.SalaryType
{
    public class CreateSalaryTypeDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string SubType { get; set; } = string.Empty;
        
        [Required]
        public SalaryCategory Category { get; set; }
    }
}
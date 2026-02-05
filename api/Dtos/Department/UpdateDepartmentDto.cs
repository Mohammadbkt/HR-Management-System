using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Department
{
    public class UpdateDepartmentDto
    {
         [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
    }
}
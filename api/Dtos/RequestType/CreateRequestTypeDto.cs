using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.RequestType
{
    public class CreateRequestTypeDto
    {
        [Required(ErrorMessage = "Type name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Type name must be between 2 and 50 characters")]
        public string TypeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "RequiresBalance is required")]
        public bool RequiresBalance { get; set; }
    }
}
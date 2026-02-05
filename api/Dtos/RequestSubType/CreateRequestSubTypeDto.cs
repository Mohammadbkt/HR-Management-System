using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Repositories
{
    public class CreateRequestSubTypeDto
    {
        [Required(ErrorMessage = "SubType name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "SubType name must be between 2 and 50 characters")]
        public string SubTypeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Request Type ID is required")]
        public int RequestTypeId { get; set; }
    }
}
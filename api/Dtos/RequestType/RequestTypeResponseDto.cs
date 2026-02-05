using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.RequestType
{
    public class RequestTypeResponseDto
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public bool RequiresBalance { get; set; }
    }
}
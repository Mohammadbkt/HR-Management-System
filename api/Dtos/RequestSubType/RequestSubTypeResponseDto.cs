using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.RequestSubType
{
    public class RequestSubTypeResponseDto
    {
        public int Id { get; set; }
        public string SubTypeName { get; set; } = string.Empty;
        public int RequestTypeId { get; set; }
        public string? RequestTypeName { get; set; }
    }
}
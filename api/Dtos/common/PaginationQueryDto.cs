using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.common
{
    public class PaginationQueryDto
    {
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
    
}
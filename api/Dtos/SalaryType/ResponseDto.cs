using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;

namespace HRMS.api.Dtos.SalaryType
{
    public class ResponseDto
    {
        public int Id { get; set; }
        public SalaryCategory Category { get; set; }
        public string SubType { get; set; } = string.Empty;
    }
}
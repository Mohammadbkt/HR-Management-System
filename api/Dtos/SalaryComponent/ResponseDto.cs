using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;

namespace HRMS.api.Dtos.SalaryComponent
{
    public class ResponseDto
    {
        
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int SalaryTypeId { get; set; }
    public string SalaryTypeName { get; set; } = string.Empty;
    public SalaryCategory Category { get; set; }
    public decimal Amount { get; set; }
    }
}
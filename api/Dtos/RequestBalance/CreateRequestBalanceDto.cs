using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.RequestBalance
{
    public class CreateRequestBalanceDto
    {
        public int EmployeeId { get; set; }
        public int RequestTypeId { get; set; }
        public int Year { get; set; }
        public decimal TotalDays { get; set; }
        public decimal UsedDays { get; set; }
        public decimal RemainingDays { get; set; }
    }
}
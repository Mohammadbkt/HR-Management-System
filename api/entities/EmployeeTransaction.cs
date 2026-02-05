using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class EmployeeTransaction
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
        public int? ManagerId { get; set; }
        public int? RequestId { get; set; }
        public int? NumberOfDays { get; set; }

        public Employee Employee { get; set; } = null!;
        public Employee? Manager { get; set; }
        public Request? Request { get; set; }


    }
}
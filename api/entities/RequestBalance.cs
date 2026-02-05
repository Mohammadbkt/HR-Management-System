    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace HRMS.api.entities
    {
        public class RequestBalance
        {
            public int EmployeeId { get; set; }
            public int RequestTypeId { get; set; }
            public int Year { get; set; }
            public decimal TotalDays { get; set; }
            public decimal UsedDays { get; set; }
            public decimal RemainingDays { get; set; }
            public DateTime? LastUpdated { get; set; }
            public Employee Employee { get; set; } = null!;
            public RequestType RequestType { get; set; } = null!;
        }
        
    }
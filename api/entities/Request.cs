using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class Request
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
        public RequestStatus Status { get; set; }
        public string? Notes { get; set; }
        public int RequestTypeId { get; set; }
        public int RequestSubTypeId { get; set; }

        public Employee Employee { get; set; } = null!;
        public RequestType RequestType { get; set; } = null!;
        public RequestSubType RequestSubType { get; set; } = null!;
        public ICollection<EmployeeTransaction> Transactions { get; set; } = new List<EmployeeTransaction>();

    }

    public enum RequestStatus
    {
        pending,
        Approved,
        Rejected,
        Cancelled
    }
}
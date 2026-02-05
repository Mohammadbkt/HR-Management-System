using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;

namespace HRMS.api.Dtos.Request
{
    public class ResponseDto
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

    }
}
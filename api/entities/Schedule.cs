using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class Schedule
    {
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
        public DateTime ShiftDate { get; set; }

        public Employee Employee { get; set; } = null!;
        public Shift Shift { get; set; } = null!;

    }
}
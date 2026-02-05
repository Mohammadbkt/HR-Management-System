using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Schedule
{
    public class CreateScheduleRequestDto
    {
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
        public DateTime ShiftDate { get; set; }


    }
}
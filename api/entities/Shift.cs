using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class Shift
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal RequiredHours { get; set; } = 8m;

        //public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        //public ICollection<Overtime> Overtimes { get; set; } = new List<Overtime>();
        // public ICollection<Shortage> Shortages { get; set; } = new List<Shortage>();

    }
}
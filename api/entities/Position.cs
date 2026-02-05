using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal DefaultBaseSalary { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }    
        public Department? Department { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class RequestType
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;

        public ICollection<RequestSubType> SubTypes { get; set; } = new List<RequestSubType>();
        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public bool RequiresBalance { get; set; }

    }
}
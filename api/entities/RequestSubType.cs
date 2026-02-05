using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class RequestSubType
    {
        public int Id { get; set; }
        public string SubTypeName { get; set; } = string.Empty;
        public int RequestTypeId { get; set; }

        public RequestType RequestType { get; set; } = null!;
        public ICollection<Request> Requests { get; set; } = new List<Request>();

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Auth
{
    public class CreateOtpRequestDto
    {
        public int UserId {get;set;}
        public string OtpHash {get;set;}
        public DateTime ExpiresAt {get;set;}
        public DateTime CreatedAt {get;set;}
        public bool IsUsed {get;set;}

    }
}
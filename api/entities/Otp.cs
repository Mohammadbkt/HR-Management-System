using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.entities
{
    public class Otp
    {
        public int Id {get;set;}
        public int UserId {get;set;}
        public string OtpHash {get;set;}
        public DateTime ExpiresAt {get;set;}
        public DateTime CreatedAt {get;set;}
        public bool IsUsed {get;set;}
        public int FailedAttempts {get;set;} = 0;

        public User user {get;set;}

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Auth
{
    public class ChangePasswordRequestDto
    {
        public string OldPassword {get;set;}
        public string NewPassword {get;set;}
    }
}
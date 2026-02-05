using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Auth
{
    public class LoginRequestDto
    {
        [EmailAddress]
        [Required]
        public string Email {get;set;}

        [Required]
        public string Password {get;set;}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Auth
{
    public class VerifyOtpResponseDto
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? Token { get; set; } 
        public DateTime? ExpiresAt {get; set;}
    }
}
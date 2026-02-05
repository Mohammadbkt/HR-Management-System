using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Auth
{
    public class AuthResponseDto
    {

        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Role { get; set; }
        public DateTime Expiration { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Errors { get; set; } = string.Empty;
    }
}
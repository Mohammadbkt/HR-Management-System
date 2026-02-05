using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.api.Dtos.Auth
{
    public class VerifyOtpRequestDto
    {

    [Required]
    public int UserId { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "OTP must contain only numbers")]
        public string OtpCode { get; set; } = string.Empty;
    }   
}
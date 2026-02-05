using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;

namespace HRMS.api.Dtos.Auth
{
    public class RegisterRequestDto
    {
         [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string UserName {get;set;} = string.Empty;
        
        public string? PhoneNumber {get;set;}
        public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
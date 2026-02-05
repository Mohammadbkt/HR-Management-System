using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Auth;
using HRMS.api.entities;
using Microsoft.AspNetCore.Identity;
using HRMS.api.Services;

namespace HRMS.api.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<AuthResponseDto> Register(RegisterRequestDto registerRequestDto);
        Task<VerifyOtpResponseDto> VerifyOtp(int userId, string OtpCode);
        Task<AuthResponseDto> ResendOtpAsync(int userId);
    }
    
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, IEmailService emailService, IOtpService otpService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailService = emailService;
            _otpService = otpService;
        }

        public async Task<AuthResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Expiration = DateTime.UtcNow,
                    Errors = "Invalid email or password"
                };
            }

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Expiration = DateTime.UtcNow,
                    Errors = "Please verify your email before logging in"
                };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Expiration = DateTime.UtcNow,
                    Errors = "Account is deactivated. Contact support."
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, false);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Expiration = DateTime.UtcNow,
                    Errors = "Invalid email or password"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expires) = _tokenService.GenerateToken(user.Id, user.UserName ?? string.Empty, roles.ToList());

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email,
                Expiration = expires,
                Role = roles.ToList()
            };
        }

        public async Task<AuthResponseDto> Register(RegisterRequestDto registerRequestDto)
        {
            var existingEmail = await _userManager.FindByEmailAsync(registerRequestDto.Email);
            if (existingEmail != null)
            {
                if (!existingEmail.EmailConfirmed)
                {
                    try
                    {
                        var otp = await _otpService.GenerateOtpAsync(existingEmail.Id);
                        await _emailService.SendEmailAsync(
                            existingEmail.Email!, 
                            "OTP Verification", 
                            $@"Hello {existingEmail.FullName},
                            Your verification code is: {otp}                            
                            This code will expire in 15 minutes."
                        );

                        return new AuthResponseDto
                        {
                            Success = false,
                            Errors = "Account exists but not verified. New OTP sent to your email."
                        };
                    }
                    catch (Exception)
                    {
                        return new AuthResponseDto
                        {
                            Success = false,
                            Errors = "Failed to send verification email. Please try again."
                        };
                    }
                }

                return new AuthResponseDto
                {
                    Success = false,
                    Expiration = DateTime.UtcNow,
                    Errors = "Email already registered"
                };
            }
            
            var existingUserName = await _userManager.FindByNameAsync(registerRequestDto.UserName);
            if (existingUserName != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Expiration = DateTime.UtcNow,
                    Errors = "Username already taken"
                };
            }

            var user = new User
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.Email,
                FullName = registerRequestDto.FullName,
                PhoneNumber = registerRequestDto.PhoneNumber,
                IsActive = false,
                EmailConfirmed = false 
            };

            var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Expiration = DateTime.UtcNow,
                    Errors = "Error while creating the user"
                };
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            // Generate and send OTP
            try
            {
                var otp = await _otpService.GenerateOtpAsync(user.Id);
                await _emailService.SendEmailAsync(
                    user.Email, 
                    "OTP Verification",
                    $@"Hello {user.FullName},
                    Welcome to HRMS! Your verification code is: {otp}
                    This code will expire in 15 minutes."
                );
            }
            catch (Exception)
            {
                await _userManager.DeleteAsync(user);
                
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = "Failed to send verification email. Please try again."
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = new List<string> { "User" }
            };
        }

        public async Task<VerifyOtpResponseDto> VerifyOtp(int userId, string otpCode)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new VerifyOtpResponseDto
                {
                    Success = false,
                    Error = "User not found"
                };
            }

            var isVerified = await _otpService.VerifyOtpAsync(user.Id, otpCode);
            if (!isVerified)
            {
                return new VerifyOtpResponseDto
                {
                    Success = false,
                    Error = "Invalid or expired OTP"
                };
            }

            user.IsActive = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            
            // Generate JWT token
            var roles = await _userManager.GetRolesAsync(user);
            var (token, expires) = _tokenService.GenerateToken(user.Id, user.UserName ?? string.Empty, roles.ToList());
        
            return new VerifyOtpResponseDto 
            { 
                Success = true, 
                Token = token,
                ExpiresAt = expires
            };
        }

        public async Task<AuthResponseDto> ResendOtpAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = "User not found"
                };
            }
            
            if (user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = "Email already verified"
                };
            }

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user.Id);
                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Resend OTP",
                    $@"Hello {user.FullName},
                    Your new verification code is: {otp}
                    This code will expire in 15 minutes."
                );
                
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "New OTP sent to your email"
                };
            }
            catch (Exception)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = "Failed to resend OTP. Please try again."
                };
            }
        }
    }
}
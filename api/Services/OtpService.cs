using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HRMS.api.Dtos.Auth;
using HRMS.api.entities;
using HRMS.api.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.api.Services
{
    public interface IOtpService
    {
        public Task<string> GenerateOtpAsync(int userId);
        public Task<bool> VerifyOtpAsync(int userId, string otpCode);
        
    }
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _otpRepository;
        public OtpService(IOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }
        public async Task<string> GenerateOtpAsync(int userId)
        {

            var existingOtp = await _otpRepository.GetOtpByUserIdAsync(userId);
            if(existingOtp != null)
            {
                await _otpRepository.DeleteOtpAsync(existingOtp.Id);
            }

            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            var value = BitConverter.ToUInt32(bytes, 0);
            var otpCode = (value % 900000 + 100000).ToString();

            var otp = new Otp()
            {
                UserId = userId,
                OtpHash = HashOtp(otpCode),
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false,
                FailedAttempts = 0
            };

            await _otpRepository.CreateOtpAsync(otp);

            return otpCode;
        }

        

        public async Task<bool> VerifyOtpAsync(int userId, string otpCode)
        {
            var otp = await _otpRepository.GetOtpByUserIdAsync(userId);

            if(otp == null) return false;
            if(otp.IsUsed) return false;
            if(otp.ExpiresAt <= DateTime.UtcNow)return false;


            if(otp.FailedAttempts >= 3)
            {
                otp.IsUsed = true;
                await _otpRepository.UpdateOtpAsync(otp);
                return false;
            }
            
            var OtpHash = HashOtp(otpCode);

            if(CryptographicOperations.FixedTimeEquals
            (
                Convert.FromBase64String(OtpHash),
                Convert.FromBase64String(otp.OtpHash)
            ))
            {
                otp.IsUsed = true;
                await _otpRepository.UpdateOtpAsync(otp);
                return true;
            }

            otp.FailedAttempts++;
            await _otpRepository.UpdateOtpAsync(otp);
            return false;
        }

        private static string HashOtp(string otp)
        {
            return Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(otp)));
        }



    }
}
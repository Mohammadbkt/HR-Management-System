using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.Dtos.Auth;
using HRMS.api.entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IOtpRepository
    {
        public Task<Otp> CreateOtpAsync(Otp otp);
        public Task<Otp?> GetOtpByUserIdAsync(int userId);
        public Task CleanupExpiredOtpsAsync();
        public Task UpdateOtpAsync(Otp otp);
        public Task DeleteOtpAsync(int otpId);

    }


    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _context;
        public OtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CleanupExpiredOtpsAsync()
        {
            var otps = await _context.Otps
                .Where(o => o.IsUsed || o.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            _context.Otps.RemoveRange(otps);            
            await _context.SaveChangesAsync();
        }

        public async Task<Otp> CreateOtpAsync(Otp otp)
        {
            await _context.Otps.AddAsync(otp);
            await _context.SaveChangesAsync();
            return otp;

        }

        public async Task DeleteOtpAsync(int otpId)
        {
            var otp = await _context.Otps.FindAsync(otpId);
            if(otp != null)
            {
                _context.Otps.Remove(otp);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Otp?> GetOtpByUserIdAsync(int userId)
        {
            return await _context.Otps
            .Where(o=>o.UserId == userId)
            .Where(o=>o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o=>o.CreatedAt)
            .FirstOrDefaultAsync(o=>o.IsUsed == false);
            
        }

        public async Task UpdateOtpAsync(Otp otp)
        {

            await _context.SaveChangesAsync();
        }
    }
}
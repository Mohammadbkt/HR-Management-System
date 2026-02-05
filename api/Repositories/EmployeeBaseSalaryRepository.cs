using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
     public interface IEmployeeBaseSalaryRepository
    {
        Task<EmployeeBaseSalary> CreateAsync(EmployeeBaseSalary baseSalary);
        Task<EmployeeBaseSalary?> GetByIdAsync(int id);
        Task<EmployeeBaseSalary?> GetCurrentByEmployeeIdAsync(int employeeId);
        Task<EmployeeBaseSalary?> GetByEmployeeIdAndDateAsync(int employeeId, DateOnly date);
        Task<List<EmployeeBaseSalary>> GetAllByEmployeeIdAsync(int employeeId);
        Task<List<EmployeeBaseSalary>> GetAllAsync();
        Task<List<EmployeeBaseSalary>> GetHistoricalByEmployeeIdAsync(int employeeId);
        Task<EmployeeBaseSalary?> UpdateAsync(EmployeeBaseSalary baseSalary);
        Task<bool> SetEndDateAsync(int id, DateOnly endDate);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasOverlappingDateRangeAsync(int employeeId, DateOnly from, DateOnly? to, int? excludeId = null);
        Task<bool> HasActiveBaseSalaryAsync(int employeeId);
    }

    public class EmployeeBaseSalaryRepository : IEmployeeBaseSalaryRepository
    {
        private readonly AppDbContext _context;

        public EmployeeBaseSalaryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeBaseSalary> CreateAsync(EmployeeBaseSalary baseSalary)
        {
            await _context.EmployeeBaseSalaries.AddAsync(baseSalary);
            await _context.SaveChangesAsync();
            return baseSalary;
        }

        public async Task<EmployeeBaseSalary?> GetByIdAsync(int id)
        {
            return await _context.EmployeeBaseSalaries
                .Include(ebs => ebs.Employee)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(ebs => ebs.Id == id);
        }

        public async Task<EmployeeBaseSalary?> GetCurrentByEmployeeIdAsync(int employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            
            return await _context.EmployeeBaseSalaries
                .Include(ebs => ebs.Employee)
                .AsNoTracking()
                .Where(ebs => ebs.EmployeeId == employeeId)
                .Where(ebs => ebs.EffectiveFrom <= today && 
                            (ebs.EffectiveTo == null || ebs.EffectiveTo >= today))
                .OrderByDescending(ebs => ebs.EffectiveFrom)
                .FirstOrDefaultAsync();
        }

        public async Task<EmployeeBaseSalary?> GetByEmployeeIdAndDateAsync(int employeeId, DateOnly date)
        {
            return await _context.EmployeeBaseSalaries
                .Include(ebs => ebs.Employee)
                .AsNoTracking()
                .Where(ebs => ebs.EmployeeId == employeeId)
                .Where(ebs => ebs.EffectiveFrom <= date && 
                            (ebs.EffectiveTo == null || ebs.EffectiveTo >= date))
                .OrderByDescending(ebs => ebs.EffectiveFrom)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EmployeeBaseSalary>> GetAllByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeBaseSalaries
                .Include(ebs => ebs.Employee)
                .AsNoTracking()
                .Where(ebs => ebs.EmployeeId == employeeId)
                .OrderByDescending(ebs => ebs.EffectiveFrom)
                .ToListAsync();
        }

        public async Task<List<EmployeeBaseSalary>> GetHistoricalByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeBaseSalaries
                .Include(ebs => ebs.Employee)
                .AsNoTracking()
                .Where(ebs => ebs.EmployeeId == employeeId && ebs.EffectiveTo != null)
                .OrderByDescending(ebs => ebs.EffectiveFrom)
                .ToListAsync();
        }

        public async Task<List<EmployeeBaseSalary>> GetAllAsync()
        {
            return await _context.EmployeeBaseSalaries
                .Include(ebs => ebs.Employee)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .OrderBy(ebs => ebs.EmployeeId)
                .ThenByDescending(ebs => ebs.EffectiveFrom)
                .ToListAsync();
        }

        public async Task<EmployeeBaseSalary?> UpdateAsync(EmployeeBaseSalary baseSalary)
        {
            var existing = await _context.EmployeeBaseSalaries
                .FirstOrDefaultAsync(ebs => ebs.Id == baseSalary.Id);

            if (existing == null)
                return null;

            existing.BaseSalary = baseSalary.BaseSalary;
            existing.EffectiveTo = baseSalary.EffectiveTo;
            
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> SetEndDateAsync(int id, DateOnly endDate)
        {
            var baseSalary = await _context.EmployeeBaseSalaries
                .FirstOrDefaultAsync(ebs => ebs.Id == id);

            if (baseSalary == null)
                return false;

            baseSalary.EffectiveTo = endDate;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var baseSalary = await _context.EmployeeBaseSalaries
                .FirstOrDefaultAsync(ebs => ebs.Id == id);

            if (baseSalary == null)
                return false;

            _context.EmployeeBaseSalaries.Remove(baseSalary);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.EmployeeBaseSalaries
                .AnyAsync(ebs => ebs.Id == id);
        }

        public async Task<bool> HasOverlappingDateRangeAsync(int employeeId, DateOnly from, DateOnly? to, int? excludeId = null)
        {
            var query = _context.EmployeeBaseSalaries
                .Where(ebs => ebs.EmployeeId == employeeId);

            if (excludeId.HasValue)
            {
                query = query.Where(ebs => ebs.Id != excludeId.Value);
            }

            var hasOverlap = await query.AnyAsync(ebs =>
                from <= (ebs.EffectiveTo ?? DateOnly.MaxValue) &&
                (to ?? DateOnly.MaxValue) >= ebs.EffectiveFrom
            );

            return hasOverlap;
        }

        public async Task<bool> HasActiveBaseSalaryAsync(int employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            
            return await _context.EmployeeBaseSalaries
                .AnyAsync(ebs => ebs.EmployeeId == employeeId &&
                                ebs.EffectiveFrom <= today &&
                                (ebs.EffectiveTo == null || ebs.EffectiveTo >= today));
        }
    }
}
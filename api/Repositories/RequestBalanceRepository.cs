using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IRequestBalanceRepository
    {
        Task<RequestBalance> CreateBalanceAsync(RequestBalance balance);
        Task<RequestBalance?> GetBalanceAsync(int employeeId, int requestTypeId, int year);
        Task<List<RequestBalance>> GetAllBalancesByEmployeeIdAsync(int employeeId, int year);
        Task<List<RequestBalance>> GetAllBalancesAsync();
        Task<RequestBalance?> UpdateBalanceAsync(RequestBalance balance);
        Task<bool> DeleteBalanceAsync(int employeeId, int requestTypeId, int year);
        Task<bool> BalanceExistsAsync(int employeeId, int requestTypeId, int year);
        Task<bool> DeductBalanceAsync(int employeeId, int requestTypeId, int year, decimal days);
        Task<bool> RestoreBalanceAsync(int employeeId, int requestTypeId, int year, decimal days);
        Task<List<RequestBalance>> GetBalancesByYearAsync(int year);
    }

    public class RequestBalanceRepository : IRequestBalanceRepository
    {
        private readonly AppDbContext _context;

        public RequestBalanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RequestBalance> CreateBalanceAsync(RequestBalance balance)
        {
            balance.RemainingDays = balance.TotalDays - balance.UsedDays;
            balance.LastUpdated = DateTime.UtcNow;

            await _context.RequestBalances.AddAsync(balance);
            await _context.SaveChangesAsync();
            return balance;
        }

        public async Task<RequestBalance?> GetBalanceAsync(int employeeId, int requestTypeId, int year)
        {
            return await _context.RequestBalances
                .Include(rb => rb.Employee)
                    .ThenInclude(e => e.User)
                .Include(rb => rb.RequestType)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(rb => 
                    rb.EmployeeId == employeeId && 
                    rb.RequestTypeId == requestTypeId && 
                    rb.Year == year);
        }

        public async Task<List<RequestBalance>> GetAllBalancesByEmployeeIdAsync(int employeeId, int year)
        {
            return await _context.RequestBalances
                .Include(rb => rb.Employee)
                    .ThenInclude(e => e.User)
                .Include(rb => rb.RequestType)
                .Where(rb => rb.EmployeeId == employeeId && rb.Year == year)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<RequestBalance>> GetAllBalancesAsync()
        {
            return await _context.RequestBalances
                .Include(rb => rb.Employee)
                    .ThenInclude(e => e.User)
                .Include(rb => rb.RequestType)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<RequestBalance>> GetBalancesByYearAsync(int year)
        {
            return await _context.RequestBalances
                .Include(rb => rb.Employee)
                    .ThenInclude(e => e.User)
                .Include(rb => rb.RequestType)
                .Where(rb => rb.Year == year)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<RequestBalance?> UpdateBalanceAsync(RequestBalance balance)
        {
            var existingBalance = await _context.RequestBalances
                .FirstOrDefaultAsync(rb => 
                    rb.EmployeeId == balance.EmployeeId && 
                    rb.RequestTypeId == balance.RequestTypeId && 
                    rb.Year == balance.Year);

            if (existingBalance == null)
                return null;

            existingBalance.TotalDays = balance.TotalDays;
            existingBalance.UsedDays = balance.UsedDays;
            existingBalance.RemainingDays = balance.TotalDays - balance.UsedDays;
            existingBalance.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingBalance;
        }

        public async Task<bool> DeleteBalanceAsync(int employeeId, int requestTypeId, int year)
        {
            var balance = await _context.RequestBalances
                .FirstOrDefaultAsync(rb => 
                    rb.EmployeeId == employeeId && 
                    rb.RequestTypeId == requestTypeId && 
                    rb.Year == year);

            if (balance == null)
                return false;

            _context.RequestBalances.Remove(balance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BalanceExistsAsync(int employeeId, int requestTypeId, int year)
        {
            return await _context.RequestBalances
                .AnyAsync(rb => 
                    rb.EmployeeId == employeeId && 
                    rb.RequestTypeId == requestTypeId && 
                    rb.Year == year);
        }

        public async Task<bool> DeductBalanceAsync(int employeeId, int requestTypeId, int year, decimal days)
        {
            var balance = await _context.RequestBalances
                .FirstOrDefaultAsync(rb => 
                    rb.EmployeeId == employeeId && 
                    rb.RequestTypeId == requestTypeId && 
                    rb.Year == year);

            if (balance == null)
                return false;

            if (balance.RemainingDays < days)
                return false;

            balance.UsedDays += days;
            balance.RemainingDays -= days;
            balance.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreBalanceAsync(int employeeId, int requestTypeId, int year, decimal days)
        {
            var balance = await _context.RequestBalances
                .FirstOrDefaultAsync(rb => 
                    rb.EmployeeId == employeeId && 
                    rb.RequestTypeId == requestTypeId && 
                    rb.Year == year);

            if (balance == null)
                return false;

            balance.UsedDays -= days;
            balance.RemainingDays += days;
            balance.LastUpdated = DateTime.UtcNow;

            if (balance.UsedDays < 0)
                balance.UsedDays = 0;

            if (balance.RemainingDays > balance.TotalDays)
                balance.RemainingDays = balance.TotalDays;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
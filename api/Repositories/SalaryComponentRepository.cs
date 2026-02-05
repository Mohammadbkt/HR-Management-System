using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface ISalaryComponentRepository
    {
        Task<SalaryComponent> AddAsync(SalaryComponent salaryComponent);
        Task<bool> DeleteAsync(int id);
        Task<SalaryComponent> UpdateAsync(int id, SalaryComponent salaryComponent);
        Task<List<SalaryComponent>> GetAllAsync();
        Task<SalaryComponent?> GetByIdAsync(int id);
        Task<List<SalaryComponent>> GetAllByEmployeeIdAsync(int employeeId);
        Task<List<SalaryComponent>> GetByEmployeeAndSalaryTypeAsync(int employeeId, int salaryTypeId);
        Task<List<SalaryComponent>> GetByEmployeeIdWithDetailsAsync(int employeeId);
        Task<decimal> GetTotalAllowancesByEmployeeIdAsync(int employeeId);
        Task<decimal> GetTotalDeductionsByEmployeeIdAsync(int employeeId);
        Task<decimal> GetNetSalaryByEmployeeIdAsync(int employeeId);
        Task<bool> ExistsAsync(int id);
    }

    public class SalaryComponentRepository : ISalaryComponentRepository
    {
        private readonly AppDbContext _context;

        public SalaryComponentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SalaryComponent> AddAsync(SalaryComponent salaryComponent)
        {
            await _context.SalaryComponents.AddAsync(salaryComponent);
            await _context.SaveChangesAsync();
            return salaryComponent;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exist = await _context.SalaryComponents.FindAsync(id);
            if(exist == null)
                return false;

            _context.SalaryComponents.Remove(exist);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.SalaryComponents.AnyAsync(s=>s.Id == id);
            
        }

        public async Task<List<SalaryComponent>> GetAllAsync()
        {
            return await _context.SalaryComponents
                            .Include(sc => sc.Employee)
                                .ThenInclude(e=>e.User)
                            .Include(sc => sc.SalaryType)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<SalaryComponent>> GetAllByEmployeeIdAsync(int employeeId)
        {
            return await _context.SalaryComponents
                        .AsNoTracking()
                        .Where(s=>s.EmployeeId == employeeId)
                        .ToListAsync();
            
        }
        public async Task<List<SalaryComponent>> GetByEmployeeAndSalaryTypeAsync(int employeeId, int salaryTypeId)
        {
            return await _context.SalaryComponents
                        .Include(s=>s.Employee)
                        .Include(s=>s.SalaryType)
                        .AsNoTracking()
                        .Where(s=>s.EmployeeId == employeeId && s.SalaryTypeId == salaryTypeId)
                        .ToListAsync();
        }

        public async Task<List<SalaryComponent>> GetByEmployeeIdWithDetailsAsync(int employeeId)
        {
            return await _context.SalaryComponents
                        .Include(s=>s.Employee)
                        .Include(s=>s.SalaryType)
                        .AsNoTracking()
                        .Where(s=>s.EmployeeId == employeeId)
                        .ToListAsync();
        }

        public async Task<SalaryComponent?> GetByIdAsync(int id)
        {
            return await _context.SalaryComponents
                            .Include(s=>s.Employee)
                            .Include(s=>s.SalaryType)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(s=> s.Id == id);

        }

        public async Task<decimal> GetNetSalaryByEmployeeIdAsync(int employeeId)
        {
            var allowances = await GetTotalAllowancesByEmployeeIdAsync(employeeId);
            var deductions = await GetTotalDeductionsByEmployeeIdAsync(employeeId);
            return allowances - deductions;
        }

        public async Task<decimal> GetTotalAllowancesByEmployeeIdAsync(int employeeId)
        {
            return await _context.SalaryComponents
                        .Include(s=>s.SalaryType)
                        .Where(s=>s.EmployeeId == employeeId && s.SalaryType.Category == SalaryCategory.Allowance)
                        .SumAsync(s=>s.Amount);
        }

        public async Task<decimal> GetTotalDeductionsByEmployeeIdAsync(int employeeId)
        {
            return await _context.SalaryComponents
                        .Include(s=>s.SalaryType)
                        .Where(s=>s.EmployeeId == employeeId && s.SalaryType.Category == SalaryCategory.Deduction)
                        .SumAsync(s=>s.Amount);
        
        }

        public async Task<SalaryComponent> UpdateAsync(int id, SalaryComponent salaryComponent)
        {
            _context.SalaryComponents.Update(salaryComponent);
            await _context.SaveChangesAsync();
            return salaryComponent;
        }

    }
}
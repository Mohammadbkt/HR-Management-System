using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface ISalaryTypesRepository
    {
        Task<SalaryType> CreateAsync(SalaryType type);
        Task<bool> DeleteAsync(int salaryTypeId);
        Task<List<SalaryType>> GetAllAsync();
        Task<SalaryType?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> SubTypeExistsAsync(string subType, SalaryCategory category);
    
    }

    public class SalaryTypesRepository : ISalaryTypesRepository
    {
        private readonly AppDbContext _context;

        public SalaryTypesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SalaryType> CreateAsync(SalaryType type)
        {
            await _context.SalaryTypes.AddAsync(type);
            await _context.SaveChangesAsync();
            return type;
        }

        public async Task<bool> DeleteAsync(int salaryTypeId)
        {
            var type = await _context.SalaryTypes.FindAsync(salaryTypeId);
            if (type == null)
                return false;


            _context.SalaryTypes.Remove(type);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.SalaryTypes.AnyAsync(s=>s.Id == id);
        }

        public async Task<List<SalaryType>> GetAllAsync()
        {
            var types = await _context.SalaryTypes
                                .AsNoTracking()
                                .ToListAsync();

            return types;
        }

        public async Task<SalaryType?> GetByIdAsync(int id)
        {
            var type =  await _context.SalaryTypes.FindAsync(id);
            
            return type;
        }

        public async Task<bool> SubTypeExistsAsync(string subType, SalaryCategory category)
        {
            
            return await _context.SalaryTypes.AnyAsync(s=>s.Category.ToString().ToLower() == category.ToString().ToLower() && s.SubType.ToLower() == subType.ToLower());
        }
    }
}
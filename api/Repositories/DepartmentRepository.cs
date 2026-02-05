using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<Department> CreateAsync(Department department);
        Task<Department?> UpdateAsync(int id, Department department);

        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;
        
        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }
    
        public async Task<Department> CreateAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return false;
            
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            
            return await _context.Departments.AnyAsync(d => d.Id == id);
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department?> UpdateAsync(int id, Department department)
        {
            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) return null;
            
            existing.Name = department.Name;
            existing.ManagerId = department.ManagerId;
            
            await _context.SaveChangesAsync();
            return existing;

        }
    }
}
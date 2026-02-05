using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.Dtos.common;
using HRMS.api.Dtos.Employee;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IEmployeeRepository
    {        
        Task<Employee?> GetEmployeeByUserIdAsync(int userId);
        Task<Employee?> GetEmployeeWithDetailsAsync(int userId);
        Task<PagedResponse<Employee>> GetAllEmployeesAsync(EmployeeQueryDto query);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int userId);
        Task<bool> EmployeeExistsAsync(int userId);
    }
    
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int userId)
        {
            var employee = await GetEmployeeByUserIdAsync(userId);
            if (employee == null)
                return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmployeeExistsAsync(int userId)
        {
            return await _context.Employees.AnyAsync(e => e.UserId == userId);
        }

        public async Task<Employee?> GetEmployeeByUserIdAsync(int userId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<Employee?> GetEmployeeWithDetailsAsync(int userId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                    .ThenInclude(m => m.User)
                .Include(e => e.BaseSalary)
                .Include(e => e.SalaryComponents)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<PagedResponse<Employee>> GetAllEmployeesAsync(EmployeeQueryDto query)
        {
            var employeesQuery = _context.Employees
                .Include(e => e.User)
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                    .ThenInclude(m => m.User)
                .AsNoTracking();    


            if (query.DepartmentId.HasValue)
                employeesQuery = employeesQuery.Where(e => e.DepartmentId == query.DepartmentId);

            if (query.PositionId.HasValue)
                employeesQuery = employeesQuery.Where(e => e.PositionId == query.PositionId);

            if (query.IsActive.HasValue)
                employeesQuery = employeesQuery.Where(e => e.User!.IsActive == query.IsActive.Value);


            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var searchLower = query.Search.ToLower();
                employeesQuery = employeesQuery.Where(e =>
                    e.User!.UserName!.ToLower().Contains(searchLower) ||
                    e.User.FullName.ToLower().Contains(searchLower));
            }

            var totalCount = await employeesQuery.CountAsync();

            employeesQuery = employeesQuery.OrderBy(e => e.User!.FullName);

            var pagedEmployees = await employeesQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();


            return new PagedResponse<Employee>
            {
                Items = pagedEmployees,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        
    }
}
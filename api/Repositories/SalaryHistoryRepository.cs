using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;

namespace HRMS.api.Repositories
{
    public interface ISalaryHistoryRepository
    {
        Task<SalaryHistory> AddAsync(SalaryHistory salaryComponent);
        Task<bool> DeleteAsync(int id);
        Task<SalaryHistory> UpdateAsync(int id, SalaryHistory salaryComponent);
        Task<List<SalaryHistory>> GetAllAsync();
        Task<SalaryHistory?> GetByIdAsync(int id);
        Task<List<SalaryHistory>> GetAllByEmployeeIdAsync(int employeeId);
        Task<List<SalaryHistory>> GetByEmployeeAndSalaryTypeAsync(int employeeId, int salaryTypeId);
        Task<List<SalaryHistory>> GetByEmployeeIdWithDetailsAsync(int employeeId);
        Task<decimal> GetTotalAllowancesByEmployeeIdAsync(int employeeId);
        Task<decimal> GetTotalDeductionsByEmployeeIdAsync(int employeeId);
        Task<decimal> GetNetSalaryByEmployeeIdAsync(int employeeId);
        Task<bool> ExistsAsync(int id);
    
    }

    public class SalaryHistoryRepository
    {
        
    }
}
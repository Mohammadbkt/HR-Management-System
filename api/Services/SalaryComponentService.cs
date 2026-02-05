using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.SalaryComponent;
using HRMS.api.entities;
using HRMS.api.Repositories;

namespace HRMS.api.Services
{
    public interface ISalaryComponentService
    {
        Task<ResponseDto?> GetByIdAsync(int id);
        Task<List<ResponseDto>> GetAllAsync();
        Task<List<ResponseDto>> GetByEmployeeIdAsync(int employeeId);
        Task<ResponseDto> CreateAsync(CreateSalaryComponentDto salaryComponent);
        Task<ResponseDto> UpdateAsync(int id, UpdateSalaryComponentDto salaryComponent);
        Task<bool> DeleteAsync(int id);
        
        Task<SalaryBreakdownDto> GetDetailedSalaryAsync(int employeeId);
        Task<List<SalaryBreakdownDto>> GetAllDetailedSalaryAsync();

        Task<decimal> GetTotalAllowancesAsync(int employeeId);
        Task<decimal> GetTotalDeductionsAsync(int employeeId);
        Task<decimal> GetNetSalaryAsync(int employeeId);
    }


    public class SalaryComponentService : ISalaryComponentService
    {
        private readonly ISalaryComponentRepository _salaryComponentRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public SalaryComponentService(ISalaryComponentRepository salaryComponentRepository, IEmployeeRepository employeeRepository)
        {
            _salaryComponentRepository = salaryComponentRepository;
            _employeeRepository = employeeRepository;

        }

        public async Task<ResponseDto> CreateAsync(CreateSalaryComponentDto salaryComponent)
        {
            var employeeExists = await _employeeRepository.EmployeeExistsAsync(salaryComponent.EmployeeId);
            if (!employeeExists)
                throw new Exception($"Employee with ID {salaryComponent.EmployeeId} does not exist");

            var existingComponents = await _salaryComponentRepository
            .GetByEmployeeAndSalaryTypeAsync(salaryComponent.EmployeeId, salaryComponent.SalaryTypeId);

            if (existingComponents.Any())
                throw new Exception($"Salary component already exists for this employee and salary type");

            if (salaryComponent.Amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero", nameof(salaryComponent.Amount));
            }
            
            var newComponent = new SalaryComponent
            {
                EmployeeId = salaryComponent.EmployeeId,
                SalaryTypeId = salaryComponent.SalaryTypeId,
                Amount = salaryComponent.Amount
            };

            var created = await _salaryComponentRepository.AddAsync(newComponent);

            return new ResponseDto
            {
                Id = created.Id,
                EmployeeId = created.EmployeeId,
                SalaryTypeId = created.SalaryTypeId,
                SalaryTypeName = created.SalaryType.SubType,
                Category = created.SalaryType.Category,  
                Amount = created.Amount
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exists = await _salaryComponentRepository.GetByIdAsync(id);            
            if (exists == null)
                throw new KeyNotFoundException($"Salary component with ID {id} not found");
            

            return await _salaryComponentRepository.DeleteAsync(id);
        }

        public async Task<List<ResponseDto>> GetAllAsync()
        {
            var components =  await _salaryComponentRepository.GetAllAsync();
            return components.Select(c=>new ResponseDto
            {
                
                Id = c.Id,
                EmployeeId = c.EmployeeId,
                SalaryTypeId = c.SalaryTypeId,
                SalaryTypeName = c.SalaryType.SubType,
                Category = c.SalaryType.Category,
                Amount = c.Amount
                
            }).ToList();
        }

        
        public async Task<List<SalaryBreakdownDto>> GetAllDetailedSalaryAsync()
        {
            var allComponents = await _salaryComponentRepository.GetAllAsync();

            // Group by employee
            var employeeGroups = allComponents.GroupBy(c => c.EmployeeId);

            var breakdowns = new List<SalaryBreakdownDto>();

            foreach (var group in employeeGroups)
            {
                var employeeId = group.Key;
                var components = group.ToList();

                var allowances = components
                    .Where(sc => sc.SalaryType.Category == SalaryCategory.Allowance)
                    .Select(c => new ResponseDto
                    {
                        Id = c.Id,
                        EmployeeId = c.EmployeeId,
                        SalaryTypeId = c.SalaryTypeId,
                        SalaryTypeName = c.SalaryType.SubType,
                        Category = c.SalaryType.Category,
                        Amount = c.Amount
                    }).ToList();

                var deductions = components
                    .Where(sc => sc.SalaryType.Category == SalaryCategory.Deduction)
                    .Select(c => new ResponseDto
                    {
                        Id = c.Id,
                        EmployeeId = c.EmployeeId,
                        SalaryTypeId = c.SalaryTypeId,
                        SalaryTypeName = c.SalaryType.SubType,
                        Category = c.SalaryType.Category,
                        Amount = c.Amount
                    }).ToList();

                var totalAllowances = allowances.Sum(a => a.Amount);
                var totalDeductions = deductions.Sum(d => d.Amount);

                breakdowns.Add(new SalaryBreakdownDto
                {
                    EmployeeId = employeeId,
                    EmployeeName = components.FirstOrDefault()?.Employee?.User?.FullName ?? "",
                    Allowances = allowances,
                    Deductions = deductions,
                    TotalAllowances = totalAllowances,
                    TotalDeductions = totalDeductions,
                    NetSalary = totalAllowances - totalDeductions
                });
            }

            return breakdowns;
        }


        public async Task<List<ResponseDto>> GetByEmployeeIdAsync(int employeeId)
        {
            var components = await _salaryComponentRepository.GetByEmployeeIdWithDetailsAsync(employeeId);
            return components.Select(c=>new ResponseDto
            {
                Id = c.Id,
                EmployeeId = c.EmployeeId,
                SalaryTypeId = c.SalaryTypeId,
                SalaryTypeName = c.SalaryType.SubType,
                Category = c.SalaryType.Category,
                Amount = c.Amount
                
            }).ToList();
        }

        public async Task<ResponseDto?> GetByIdAsync(int id)
        {
            var salary = await _salaryComponentRepository.GetByIdAsync(id);
            if(salary == null)
                throw new Exception("salary component not found");
            
            return new ResponseDto
            {
                Id = salary.Id,
                EmployeeId = salary.EmployeeId,
                SalaryTypeId = salary.SalaryTypeId,
                SalaryTypeName = salary.SalaryType.SubType,
                Category = salary.SalaryType.Category,
                Amount = salary.Amount
            };
            
        }

        public async Task<SalaryBreakdownDto> GetDetailedSalaryAsync(int employeeId)
        {
            var employeeExists = await _employeeRepository.EmployeeExistsAsync(employeeId);
            if (!employeeExists)
                throw new Exception($"Employee with ID {employeeId} does not exist");
    
            var components = await _salaryComponentRepository.GetByEmployeeIdWithDetailsAsync(employeeId);
            var allowances = components
            .Where(sc => sc.SalaryType.Category == SalaryCategory.Allowance)
            .Select(c => new ResponseDto
            {
                Id = c.Id,
                EmployeeId = c.EmployeeId,
                SalaryTypeId = c.SalaryTypeId,
                SalaryTypeName = c.SalaryType.SubType,
                Category = c.SalaryType.Category,
                Amount = c.Amount
            }).ToList();
        
        var deductions = components
            .Where(sc => sc.SalaryType.Category == SalaryCategory.Deduction)
            .Select(c => new ResponseDto
            {
                Id = c.Id,
                EmployeeId = c.EmployeeId,
                SalaryTypeId = c.SalaryTypeId,
                SalaryTypeName = c.SalaryType.SubType,
                Category = c.SalaryType.Category,
                Amount = c.Amount
            }).ToList();
        
        var totalAllowances = await GetTotalAllowancesAsync(employeeId);
        var totalDeductions = await GetTotalDeductionsAsync(employeeId);

        return new SalaryBreakdownDto
        {
            EmployeeId = employeeId,
            EmployeeName = components.FirstOrDefault()?.Employee?.User?.FullName ?? "",
            Allowances = allowances,
            Deductions = deductions,
            TotalAllowances = totalAllowances,
            TotalDeductions = totalDeductions,
            NetSalary = totalAllowances - totalDeductions
        };
        
        }

        public async Task<decimal> GetNetSalaryAsync(int employeeId)
        {
            return await _salaryComponentRepository.GetNetSalaryByEmployeeIdAsync(employeeId);

        }

        public async Task<decimal> GetTotalAllowancesAsync(int employeeId)
        {
            return await _salaryComponentRepository.GetTotalAllowancesByEmployeeIdAsync(employeeId);
        }

        public async Task<decimal> GetTotalDeductionsAsync(int employeeId)
        {
            return await _salaryComponentRepository.GetTotalDeductionsByEmployeeIdAsync(employeeId);
        }

        public async Task<ResponseDto> UpdateAsync(int id, UpdateSalaryComponentDto salaryComponent)
        {
            var salary = await _salaryComponentRepository.GetByIdAsync(id);
            if(salary == null)
                throw new Exception("salary component not found");
            
            if(salaryComponent.Amount <= 0)
                throw new Exception("salary component amount must be a positive");

            salary.Amount = salaryComponent.Amount;

            var updated = await _salaryComponentRepository.UpdateAsync(id, salary);

            return new ResponseDto
            {
                Id = updated.Id,
                EmployeeId = updated.EmployeeId,
                SalaryTypeId = updated.SalaryTypeId,
                SalaryTypeName = updated.SalaryType.SubType,
                Category = updated.SalaryType.Category,
                Amount = updated.Amount
            };

        }
    }
}
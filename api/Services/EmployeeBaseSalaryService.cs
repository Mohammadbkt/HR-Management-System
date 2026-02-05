using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.EmployeeBaseSalary;
using HRMS.api.entities;
using HRMS.api.Repositories;

namespace HRMS.api.Services
{
    public interface IEmployeeBaseSalaryService
    {
        Task<ResponseDto> CreateEmployeeBaseSalaryAsync(int employeeId, CreateEmployeeBaseSalaryDto dto);
        Task<ResponseDto> UpdateEmployeeBaseSalaryAsync(int id, int employeeId, UpdateEmployeeBaseSalaryDto dto);
        Task<ResponseDto> GetEmployeeBaseSalaryByIdAsync(int id);
        Task<ResponseDto> GetCurrentByEmployeeIdAsync(int EmployeeId);
        Task<ResponseDto> GetByEmployeeIdAndDateAsync(int EmployeeId, DateOnly date);
        Task<List<ResponseDto>> GetAllByEmployeeIdAsync(int employeeId);
        Task<List<ResponseDto>> GetAllAsync();
        Task<List<ResponseDto>> GetHistoricalByEmployeeIdAsync(int employeeId);
        Task<bool> SetEndDateAsync(int id, DateOnly endDate);
        Task<bool> DeleteAsync(int id);
    }


    public class EmployeeBaseSalaryService : IEmployeeBaseSalaryService
    {
        private readonly IEmployeeBaseSalaryRepository _employeeBaseSalaryRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeBaseSalaryService(IEmployeeBaseSalaryRepository employeeBaseSalaryRepository, IEmployeeRepository employeeRepository)
        {
            _employeeBaseSalaryRepository = employeeBaseSalaryRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<ResponseDto> CreateEmployeeBaseSalaryAsync(int employeeId, CreateEmployeeBaseSalaryDto dto)
        {
            var employee = await _employeeRepository.GetEmployeeByUserIdAsync(employeeId);
            if(employee == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found");

            
            if(dto.BaseSalary < 500)
            {
                throw new ArgumentException("Base salary must be at least 500");
            }   
            
            var hasOverlap =  await _employeeBaseSalaryRepository.HasOverlappingDateRangeAsync(employeeId, dto.EffectiveFrom, dto.EffectiveTo);
            if(hasOverlap)
                throw new InvalidOperationException("Date range overlaps with existing salary record");
            
            var currentSalary = await _employeeBaseSalaryRepository.GetCurrentByEmployeeIdAsync(employeeId);
            if (currentSalary != null)
            {
                var activeMonths = (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - currentSalary.EffectiveFrom.DayNumber) / 30;
                if (activeMonths < 3)
                {
                    throw new InvalidOperationException($"Current salary must be active for at least 3 months before creating a new one. Current duration: {activeMonths} months");
                }
            }


            var baseSalary = new EmployeeBaseSalary
            {
                EmployeeId = employee.UserId,
                BaseSalary = dto.BaseSalary,
                EffectiveFrom = dto.EffectiveFrom,
                EffectiveTo = dto.EffectiveTo
            };

            var created = await _employeeBaseSalaryRepository.CreateAsync(baseSalary);
            if(created == null)
                throw new Exception("Error happend while creating an employee base salary");
            
            return new ResponseDto
            {
                Id = created.Id,
                EmployeeId = created.EmployeeId,
                BaseSalary = created.BaseSalary,
                EffectiveFrom = created.EffectiveFrom,
                EffectiveTo = created.EffectiveTo
            };
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _employeeBaseSalaryRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Base salary record with ID {id} not found");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (existing.EffectiveFrom <= today)
            {
                var activeMonths = (today.DayNumber - existing.EffectiveFrom.DayNumber) / 30;
                if (activeMonths < 3)
                {
                    throw new InvalidOperationException($"Salary record must be active for at least 3 months before deleting. Current duration: {activeMonths} months");
                }
            }

            return await _employeeBaseSalaryRepository.DeleteAsync(id);
        }

        public async Task<List<ResponseDto>> GetAllAsync()
        {
            var salaries = await _employeeBaseSalaryRepository.GetAllAsync();
            return salaries.Select(s=> new ResponseDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                BaseSalary = s.BaseSalary,
                EffectiveFrom = s.EffectiveFrom,
                EffectiveTo = s.EffectiveTo
            }).ToList();
        }

        public async Task<List<ResponseDto>> GetAllByEmployeeIdAsync(int employeeId)
        {
            var employeeExists =  await _employeeRepository.EmployeeExistsAsync(employeeId);
            if(!employeeExists)
                throw new Exception("Employee does not exists");
            
            var salaries = await _employeeBaseSalaryRepository.GetAllByEmployeeIdAsync(employeeId);

            return salaries.Select(s=>new ResponseDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                BaseSalary = s.BaseSalary,
                EffectiveFrom = s.EffectiveFrom,
                EffectiveTo = s.EffectiveTo
            }).ToList();
        }

        public async Task<ResponseDto> GetByEmployeeIdAndDateAsync(int employeeId, DateOnly date)
        {
            var employeeExists =  await _employeeRepository.EmployeeExistsAsync(employeeId);
            if(!employeeExists)
                throw new Exception("Employee does not exists");
            
            var salaries = await _employeeBaseSalaryRepository.GetByEmployeeIdAndDateAsync(employeeId, date);
            if(salaries == null)
                throw new Exception("Employee bas salary does not exists");

            return new ResponseDto
            {
            
                Id = salaries.Id,
                EmployeeId = salaries.EmployeeId,
                BaseSalary = salaries.BaseSalary,
                EffectiveFrom = salaries.EffectiveFrom,
                EffectiveTo = salaries.EffectiveTo
            };
        }

        public async Task<ResponseDto> GetCurrentByEmployeeIdAsync(int employeeId)
        {
            var employeeExists =  await _employeeRepository.EmployeeExistsAsync(employeeId);
            if(!employeeExists)
                throw new Exception("Employee does not exists");
            
            var salaries = await _employeeBaseSalaryRepository.GetCurrentByEmployeeIdAsync(employeeId);
            if(salaries == null)
                throw new Exception("Employee base salary does not exists");

            return new ResponseDto{
            
                Id = salaries.Id,
                EmployeeId = salaries.EmployeeId,
                BaseSalary = salaries.BaseSalary,
                EffectiveFrom = salaries.EffectiveFrom,
                EffectiveTo = salaries.EffectiveTo
            };
        }

        public async Task<ResponseDto> GetEmployeeBaseSalaryByIdAsync(int id)
        {
            
            var salary = await _employeeBaseSalaryRepository.GetByIdAsync(id);
            if(salary == null)
                throw new Exception("Employee base salary does not exists");

            return new ResponseDto
            {
                Id = salary.Id,
                EmployeeId = salary.EmployeeId,
                BaseSalary = salary.BaseSalary,
                EffectiveFrom = salary.EffectiveFrom,
                EffectiveTo = salary.EffectiveTo
            };
        }

        public async Task<List<ResponseDto>> GetHistoricalByEmployeeIdAsync(int employeeId)
        {
            var employeeExists =  await _employeeRepository.EmployeeExistsAsync(employeeId);
            if(!employeeExists)
                throw new Exception("Employee does not exists");

            var salaries = await _employeeBaseSalaryRepository.GetHistoricalByEmployeeIdAsync(employeeId);
            return salaries.Select(s=>new ResponseDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                BaseSalary = s.BaseSalary,
                EffectiveFrom = s.EffectiveFrom,
                EffectiveTo = s.EffectiveTo
            }).ToList();
        }

        public async Task<bool> SetEndDateAsync(int id, DateOnly endDate)
        {
            var existing = await _employeeBaseSalaryRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Base salary record with ID {id} not found");


            return await _employeeBaseSalaryRepository.SetEndDateAsync(id, endDate);
        }

        public async Task<ResponseDto> UpdateEmployeeBaseSalaryAsync(int id, int employeeId, UpdateEmployeeBaseSalaryDto dto)
        {
            var existing = await _employeeBaseSalaryRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Base salary record with ID {id} not found");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (existing.EffectiveFrom <= today)
            {
                var activeMonths = (today.DayNumber - existing.EffectiveFrom.DayNumber) / 30;
                if (activeMonths < 3)
                {
                    throw new InvalidOperationException($"Salary record must be active for at least 3 months before updating. Current duration: {activeMonths} months");
                }
            }

            if (employeeId != existing.EmployeeId)
                throw new ArgumentException("Cannot change employee ID in an update operation");

             if (dto.BaseSalary <= 0)
                throw new ArgumentException("Base salary must be greater than zero");
            

            if(dto.BaseSalary < 500)
            {
                throw new Exception("base salary must be at least 500");
            }

        
            if (dto.EffectiveTo.HasValue && dto.EffectiveTo.Value <= existing.EffectiveFrom)
                throw new ArgumentException($"Effective to date must be after effective from date ({existing.EffectiveFrom:yyyy-MM-dd})");

            var hasOverlap = await _employeeBaseSalaryRepository.HasOverlappingDateRangeAsync(existing.EmployeeId, existing.EffectiveFrom, dto.EffectiveTo, id);
            
            if (hasOverlap)
                throw new InvalidOperationException(
                    "Updated date range would overlap with another salary record");

            existing.BaseSalary = dto.BaseSalary;
            existing.EffectiveTo = dto.EffectiveTo;

            var updated = await _employeeBaseSalaryRepository.UpdateAsync(existing);
            if(updated == null)
                throw new Exception("Error happend while updating the base salary");
            
            return new ResponseDto
            {
                Id = updated.Id,
                EmployeeId = updated.EmployeeId,
                BaseSalary = updated.BaseSalary,
                EffectiveFrom = updated.EffectiveFrom,
                EffectiveTo = updated.EffectiveTo
            };


        }
    }
}
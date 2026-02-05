using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using HRMS.api.Dtos.RequestBalance;
using HRMS.api.entities;
using HRMS.api.Repositories;
using Microsoft.Build.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HRMS.api.Services
{

    public interface IRequestBalanceService
    {
        Task<RequestBalanceResponseDto> CreateBalanceAsync(CreateRequestBalanceDto dto);
        Task<RequestBalanceResponseDto> UpdateBalanceAsync(UpdateRequestBalanceDto dto);
        Task<RequestBalanceResponseDto> GetBalanceAsync(int employeeId, int requestTypeId, int year);
        Task<List<RequestBalanceResponseDto>> GetAllBalanceByEmployeeIdAsync(int employeeId, int year);
        Task<List<RequestBalanceResponseDto>> GetAllBalancesAsync();
        Task<bool> DeleteBalanceAsync(int employeeId, int requestTypeId, int year);
        Task<bool> DeductBalanceAsync(int employeeId, int requestTypeId, int year, decimal days);
        Task<bool> RestoreBalanceAsync(int employeeId, int requestTypeId, int year, decimal days);
        Task<List<RequestBalanceResponseDto>> GetBalancesByYearAsync(int year);
    }

    public class RequestBalanceService : IRequestBalanceService
    {
        private readonly IRequestBalanceRepository _requestBalanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRequestTypeRepository _requestTypeRepository;

        public RequestBalanceService(IRequestBalanceRepository requestBalanceRepository, IEmployeeRepository employeeRepository, IRequestTypeRepository requestTypeRepository)
        {
            _requestBalanceRepository = requestBalanceRepository;
            _employeeRepository = employeeRepository;
            _requestTypeRepository = requestTypeRepository;
            
        }

        public async Task<RequestBalanceResponseDto> CreateBalanceAsync(CreateRequestBalanceDto dto)
        {
            var employeeExists =await _employeeRepository.EmployeeExistsAsync(dto.EmployeeId);
            if (!employeeExists)
                throw new Exception($"Employee with ID {dto.EmployeeId} does not exist");

            var requestType = await _requestTypeRepository.GetRequestTypeByIdAsync(dto.RequestTypeId);
            if (requestType == null)
                throw new Exception($"Request type with ID {dto.RequestTypeId} does not exist");
            
            if(dto.Year > DateTime.UtcNow.Year)
                throw new Exception($"Year {dto.Year} cannot be in the future");
                

            var existingBalance  = await _requestBalanceRepository.BalanceExistsAsync(dto.EmployeeId, dto.RequestTypeId, dto.Year);
            if (existingBalance)
                throw new Exception($"Balance already exists for employee {dto.EmployeeId}, request type {dto.RequestTypeId}, year {dto.Year}");
            
             if (dto.TotalDays < 0)
                throw new ArgumentException("Total days cannot be negative");

            if (dto.UsedDays < 0)
                throw new ArgumentException("Used days cannot be negative");

            if (dto.UsedDays > dto.TotalDays)
                throw new ArgumentException("Used days cannot exceed total days");
    


            var balance = new RequestBalance
            {
                EmployeeId = dto.EmployeeId,
                RequestTypeId = dto.RequestTypeId,
                Year = dto.Year,
                TotalDays = dto.TotalDays,
                UsedDays = dto.UsedDays
            };

            var created = await _requestBalanceRepository.CreateBalanceAsync(balance);
            if(created == null)
                throw new Exception("Failed to create balance");
            
            return new RequestBalanceResponseDto
            {
                
                EmployeeId = created.EmployeeId,
                RequestTypeId = created.RequestTypeId,
                Year = created.Year,
                TotalDays = created.TotalDays,
                UsedDays = created.UsedDays,
                RemainingDays = created.RemainingDays
            };
        }

        public async Task<bool> DeductBalanceAsync(int employeeId, int requestTypeId, int year, decimal days)
        {
            if(year > DateTime.UtcNow.Year)
                throw new Exception($"Year {year} cannot be in the future");
                
            if(days <= 0)
                throw new ArgumentException("Days to deduct must be greater than zero");
        

            var deducted = await _requestBalanceRepository.DeductBalanceAsync(employeeId, requestTypeId, year, days);
            if (!deducted)
                throw new InvalidOperationException($"Failed to deduct {days} days. Balance may not exist or insufficient remaining days available.");
    
            return true;
        }

        public async Task<bool> DeleteBalanceAsync(int employeeId, int requestTypeId, int year)
        {
            var balance =  await _requestBalanceRepository.GetBalanceAsync(employeeId, requestTypeId, year);
            if(balance == null)
                throw new Exception("Faild to get the balance");

            var deleted =  await _requestBalanceRepository.DeleteBalanceAsync(employeeId, requestTypeId, year);
            return deleted;
        }

        public async Task<List<RequestBalanceResponseDto>> GetAllBalanceByEmployeeIdAsync(int employeeId, int year)
        {
            var balances = await _requestBalanceRepository.GetAllBalancesByEmployeeIdAsync(employeeId, year);

            return balances.Select(b=> new RequestBalanceResponseDto
            {
                EmployeeId = b.EmployeeId,
                RequestTypeId = b.RequestTypeId,
                Year = b.Year,
                TotalDays = b.TotalDays,//is this a good design ? or should i get it based on the position let's say 
                UsedDays = b.UsedDays,
                RemainingDays = b.RemainingDays
            }).ToList();


        }

        public async Task<List<RequestBalanceResponseDto>> GetAllBalancesAsync()
        {
            var balances =  await _requestBalanceRepository.GetAllBalancesAsync();

            return balances.Select(b=> new RequestBalanceResponseDto 
            {
                EmployeeId = b.EmployeeId,
                RequestTypeId = b.RequestTypeId,
                Year = b.Year,
                TotalDays = b.TotalDays,//is this a good design ? or should i get it based on the position let's say 
                UsedDays = b.UsedDays,
                RemainingDays = b.RemainingDays
            }).ToList();

        }

        public async Task<RequestBalanceResponseDto> GetBalanceAsync(int employeeId, int requestTypeId, int year)
        {
            var balance =  await _requestBalanceRepository.GetBalanceAsync(employeeId, requestTypeId, year);
            if(balance == null)
                throw new Exception("Faild to get the balance");

            return new RequestBalanceResponseDto
            {
                EmployeeId = balance.EmployeeId,
                RequestTypeId = balance.RequestTypeId,
                Year = balance.Year,
                TotalDays = balance.TotalDays,//is this a good design ? or should i get it based on the position ? let's say 
                UsedDays = balance.UsedDays,
                RemainingDays = balance.RemainingDays
            };
        }

        public async Task<List<RequestBalanceResponseDto>> GetBalancesByYearAsync(int year)
        {
            var balance =  await _requestBalanceRepository.GetBalancesByYearAsync(year);
            
            return balance.Select(b=> new RequestBalanceResponseDto
            {
                EmployeeId = b.EmployeeId,
                RequestTypeId = b.RequestTypeId,
                Year = b.Year,
                TotalDays = b.TotalDays,//is this a good design ? or should i get it based on the position ? let's say 
                UsedDays = b.UsedDays,
                RemainingDays = b.RemainingDays
            }).ToList(); 
        }

        public async Task<bool> RestoreBalanceAsync(int employeeId, int requestTypeId, int year, decimal days)
        {
            if(year > DateTime.UtcNow.Year)
                throw new Exception($"Year {year} cannot be in the future");
                    
             
            if(days <= 0)
                throw new ArgumentException("Days to restore must be greater than zero");
        
            var restored = await _requestBalanceRepository.RestoreBalanceAsync(employeeId, requestTypeId, year, days);

            if(!restored)
                throw new InvalidOperationException($"Failed to restore {days} days. Balance may not exist.");
    

            return true;
        }

        public async Task<RequestBalanceResponseDto> UpdateBalanceAsync(UpdateRequestBalanceDto dto)
        {
            var employeeExists =await _employeeRepository.EmployeeExistsAsync(dto.EmployeeId);
            if (!employeeExists)
                throw new Exception($"Employee with ID {dto.EmployeeId} does not exist");

            var requestType = await _requestTypeRepository.GetRequestTypeByIdAsync(dto.RequestTypeId);
            if (requestType == null)
                throw new Exception($"Request type with ID {dto.RequestTypeId} does not exist");
            
            if(dto.Year > DateTime.UtcNow.Year)
                throw new Exception($"Year {dto.Year} cannot be in the future");
                
            var balance = await _requestBalanceRepository.GetBalanceAsync(dto.EmployeeId, dto.RequestTypeId, dto.Year);
            if (balance == null)
                throw new Exception($"Balance not found for employee {dto.EmployeeId}, request type {dto.RequestTypeId}, year {dto.Year}");

            if (dto.TotalDays < 0)
                throw new ArgumentException("Total days cannot be negative");
            
            if (dto.UsedDays < 0)
                throw new ArgumentException("Used days cannot be negative");
            
            if (dto.UsedDays > dto.TotalDays)
                throw new ArgumentException("Used days cannot exceed total days");


            balance.TotalDays = dto.TotalDays;
            balance.UsedDays = dto.UsedDays;

            var updated =  await _requestBalanceRepository.UpdateBalanceAsync(balance);
            if(updated == null)
                throw new InvalidOperationException("Failed to update balance");
            
            return new RequestBalanceResponseDto
            {
                EmployeeId = updated.EmployeeId,
                RequestTypeId = updated.RequestTypeId,
                Year = updated.Year,
                TotalDays = updated.TotalDays,//is this a good design ? or should i get it based on the position ? let's say 
                UsedDays = updated.UsedDays,
                RemainingDays = updated.RemainingDays
            };
        }
    }
}
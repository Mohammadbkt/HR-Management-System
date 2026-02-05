using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.common;
using HRMS.api.Dtos.Employee;
using HRMS.api.entities;
using HRMS.api.Repositories;
using Microsoft.AspNetCore.Identity;

namespace HRMS.api.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeResponseDto> GetMyEmployeeProfileAsync(int userId);
        Task<EmployeeResponseDto> CreateEmployeeProfileAsync(int userId, CreateEmployeeRequestDto dto);
        Task<EmployeeResponseDto> UpdateEmployeeProfileAsync(int userId, UpdateEmployeeDto dto);
        Task<bool> DeleteEmployeeProfileAsync(int userId);
        Task<PagedResponse<EmployeeResponseDto>> GetAllEmployeesAsync(EmployeeQueryDto query);
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int userId);
    }
    
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<User> _userManager;

        public EmployeeService(IEmployeeRepository employeeRepository, UserManager<User> userManager)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
        }

        public async Task<EmployeeResponseDto> CreateEmployeeProfileAsync(int userId, CreateEmployeeRequestDto dto)
        {
            var exists = await _employeeRepository.EmployeeExistsAsync(userId);
            if (exists)
                throw new InvalidOperationException($"Employee profile already exists for user {userId}");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");
            
            var newEmployee = new Employee
            {
                UserId = userId,
                SSN = dto.SSN,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                StartDate = dto.StartDate ?? DateTimeOffset.UtcNow,
                ManagerId = dto.ManagerId,
                WorkEmail = dto.WorkEmail,
                WorkPhone = dto.WorkPhone,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                LastWorkingDate = dto.LastWorkingDate
            };
            
            var createdEmployee = await _employeeRepository.CreateEmployeeAsync(newEmployee);
            
            var employee = await _employeeRepository.GetEmployeeWithDetailsAsync(userId);
            if(employee == null)
                throw new Exception("Error while creating profile");


            return new EmployeeResponseDto
            {
                UserId = employee.UserId,
                
                // User information
                UserName = createdEmployee.User?.UserName,
                FullName = createdEmployee.User?.FullName,
                Email = createdEmployee.User?.Email,
                PhoneNumber = createdEmployee.User?.PhoneNumber,
                ProfilePicture = createdEmployee.User?.ProfilePicture,
                IsActive = createdEmployee.User?.IsActive ?? false,
                
                // Employee information
                SSN = createdEmployee.SSN,
                WorkEmail = createdEmployee.WorkEmail,
                WorkPhone = createdEmployee.WorkPhone,
                StartDate = createdEmployee.StartDate,
                BirthDate = createdEmployee.BirthDate,
                Gender = createdEmployee.Gender,
                LastWorkingDate = createdEmployee.LastWorkingDate,
                
                // Related entities
                ManagerId = createdEmployee.ManagerId,
                ManagerName = createdEmployee.Manager?.User?.FullName,
                
                DepartmentId = createdEmployee.DepartmentId,
                DepartmentName = createdEmployee.Department?.Name,
                
                PositionId = createdEmployee.PositionId,
                PositionTitle = createdEmployee.Position?.Title
            };
        }

        public async Task<bool> DeleteEmployeeProfileAsync(int userId)
        {
            var exists = await _employeeRepository.EmployeeExistsAsync(userId);
            if (!exists)
                return false;

            return await _employeeRepository.DeleteEmployeeAsync(userId);
        }

        public async Task<PagedResponse<EmployeeResponseDto>> GetAllEmployeesAsync(EmployeeQueryDto query)
        {
            var result = await _employeeRepository.GetAllEmployeesAsync(query);
            
            return new PagedResponse<EmployeeResponseDto>
            {
                Items = result.Items.Select(e => new EmployeeResponseDto
                {
                    UserId = e.UserId,

                    UserName = e.User?.UserName,
                    FullName = e.User?.FullName,
                    Email = e.User?.Email,
                    PhoneNumber = e.User?.PhoneNumber,
                    ProfilePicture = e.User?.ProfilePicture,
                    IsActive = e.User?.IsActive ?? false,

                    SSN = e.SSN,
                    WorkEmail = e.WorkEmail,
                    WorkPhone = e.WorkPhone,
                    StartDate = e.StartDate,
                    BirthDate = e.BirthDate,
                    Gender = e.Gender,
                    LastWorkingDate = e.LastWorkingDate,

                    ManagerId = e.ManagerId,
                    ManagerName = e.Manager?.User?.FullName,

                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department?.Name,

                    PositionId = e.PositionId,
                    PositionTitle = e.Position?.Title
                }).ToList(),

                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
            };
        }

        public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(int userId)
        {
            var employee = await _employeeRepository.GetEmployeeWithDetailsAsync(userId);
            if (employee == null)
                throw new InvalidOperationException($"Employee with user ID {userId} not found");
            
            return new EmployeeResponseDto
            {
                UserId = employee.UserId,
                
                UserName = employee.User?.UserName,
                FullName = employee.User?.FullName,
                Email = employee.User?.Email,
                PhoneNumber = employee.User?.PhoneNumber,
                ProfilePicture = employee.User?.ProfilePicture,
                IsActive = employee.User?.IsActive ?? false,
                
                SSN = employee.SSN,
                WorkEmail = employee.WorkEmail,
                WorkPhone = employee.WorkPhone,
                StartDate = employee.StartDate,
                BirthDate = employee.BirthDate,
                Gender = employee.Gender,
                LastWorkingDate = employee.LastWorkingDate,
                
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.User?.FullName,
                
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                
                PositionId = employee.PositionId,
                PositionTitle = employee.Position?.Title
            };
        }

        public async Task<EmployeeResponseDto> GetMyEmployeeProfileAsync(int userId)
        {
            var employee = await _employeeRepository.GetEmployeeWithDetailsAsync(userId);
            if (employee == null)
                throw new InvalidOperationException($"Employee profile not found for user {userId}");

            return new EmployeeResponseDto
            {
                UserId = employee.UserId,
                
                UserName = employee.User?.UserName,
                FullName = employee.User?.FullName,
                Email = employee.User?.Email,
                PhoneNumber = employee.User?.PhoneNumber,
                ProfilePicture = employee.User?.ProfilePicture,
                IsActive = employee.User?.IsActive ?? false,
                
                SSN = employee.SSN,
                WorkEmail = employee.WorkEmail,
                WorkPhone = employee.WorkPhone,
                StartDate = employee.StartDate,
                BirthDate = employee.BirthDate,
                Gender = employee.Gender,
                LastWorkingDate = employee.LastWorkingDate,
                
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.User?.FullName,
                
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                
                PositionId = employee.PositionId,
                PositionTitle = employee.Position?.Title
            };
        }

        public async Task<EmployeeResponseDto> UpdateEmployeeProfileAsync(int userId, UpdateEmployeeDto dto)
        {
            var employee = await _employeeRepository.GetEmployeeWithDetailsAsync(userId);
            if (employee == null)
                throw new InvalidOperationException($"Employee with user ID {userId} not found");

            employee.ManagerId = dto.ManagerId;
            employee.DepartmentId = dto.DepartmentId;
            employee.WorkEmail = dto.WorkEmail;
            employee.WorkPhone = dto.WorkPhone;
            employee.BirthDate = dto.BirthDate;
            employee.LastWorkingDate = dto.LastWorkingDate;
            employee.PositionId = dto.PositionId;
            
            if (!string.IsNullOrWhiteSpace(dto.SSN))
            {
                employee.SSN = dto.SSN;
            }
            
            if (!Char.IsWhiteSpace(dto.Gender))
            {
                employee.Gender = dto.Gender;
            }

            await _employeeRepository.UpdateEmployeeAsync(employee);
            
            var updatedEmployee = await _employeeRepository.GetEmployeeWithDetailsAsync(userId);
            
            return new EmployeeResponseDto
            {
                UserId = employee.UserId,
                
                UserName = employee.User?.UserName,
                FullName = employee.User?.FullName,
                Email = employee.User?.Email,
                PhoneNumber = employee.User?.PhoneNumber,
                ProfilePicture = employee.User?.ProfilePicture,
                IsActive = employee.User?.IsActive ?? false,
                
                SSN = employee.SSN,
                WorkEmail = employee.WorkEmail,
                WorkPhone = employee.WorkPhone,
                StartDate = employee.StartDate,
                BirthDate = employee.BirthDate,
                Gender = employee.Gender,
                LastWorkingDate = employee.LastWorkingDate,
                
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.User?.FullName,
                
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                
                PositionId = employee.PositionId,
                PositionTitle = employee.Position?.Title
            };;
        }

    }
}
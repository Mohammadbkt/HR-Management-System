using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using HRMS.api.Dtos.EmployeeBaseSalary;
using HRMS.api.Dtos.SalaryType;
using HRMS.api.entities;
using HRMS.api.Repositories;
using ResponseDto = HRMS.api.Dtos.SalaryType.ResponseDto;

namespace HRMS.api.Services
{
    public interface ISalaryTypesService
    {
        Task<ResponseDto> CreateAsync(CreateSalaryTypeDto dto);
        Task<bool> DeleteAsync(int salaryTypeId);
        Task<List<ResponseDto>> GetAllAsync();
        Task<ResponseDto?> GetByIdAsync(int id);
    }

    public class SalaryTypesService : ISalaryTypesService
    {
        private readonly ISalaryTypesRepository _repository;

        public SalaryTypesService(ISalaryTypesRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseDto> CreateAsync(CreateSalaryTypeDto type)
        {
            var subexist =  await _repository.SubTypeExistsAsync(type.SubType, type.Category);
            if(subexist)
                throw new InvalidOperationException($"Salary subtype '{type.SubType}' already exists in category '{type.Category}'");
            
            type.SubType = type.SubType.Trim();
            
            var typeToCreate = new SalaryType
            {
                Category = type.Category,
                SubType = type.SubType
            };


            var created = await _repository.CreateAsync(typeToCreate);

            return new ResponseDto
            {
                Id = created.Id,
                Category = created.Category,
                SubType = created.SubType
            };
        }

        public async Task<bool> DeleteAsync(int salaryTypeId)
        {
            return await _repository.DeleteAsync(salaryTypeId);
        }

        public async Task<List<ResponseDto>> GetAllAsync()
        {
            var types = await _repository.GetAllAsync();


            return types.Select(t=> new ResponseDto
            {
                Id = t.Id,
                Category = t.Category,
                SubType = t.SubType
            }).ToList();

        }

        public async Task<ResponseDto?> GetByIdAsync(int id)
        {
            var type = await _repository.GetByIdAsync(id);
            if(type == null)
                return null;

            return new ResponseDto
            {
                Id = type.Id,
                Category = type.Category,
                SubType = type.SubType
            };
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.RequestType;
using HRMS.api.entities;
using HRMS.api.Repositories;

namespace HRMS.api.Services
{
    public interface IRequestTypeService
    {
        Task<RequestTypeResponseDto> CreateRequestTypeAsync(CreateRequestTypeDto dto);
        Task<RequestTypeResponseDto?> UpdateRequestTypeAsync(int id, UpdateRequestTypeDto dto);
        Task<bool> DeleteRequestTypeAsync(int id);
        Task<RequestTypeResponseDto?> GetRequestTypeByIdAsync(int id);
        Task<List<RequestTypeResponseDto>> GetAllRequestTypesAsync();
    }

    public class RequestTypeService : IRequestTypeService
    {
        private readonly IRequestTypeRepository _requestTypeRepository;

        public RequestTypeService(IRequestTypeRepository requestTypeRepository)
        {
            _requestTypeRepository = requestTypeRepository;
        }

        public async Task<RequestTypeResponseDto> CreateRequestTypeAsync(CreateRequestTypeDto dto)
        {
            var nameExists = await _requestTypeRepository.RequestTypeNameExistsAsync(dto.TypeName);
            if (nameExists)
                throw new Exception($"Request type with name '{dto.TypeName}' already exists");

            var requestType = new RequestType
            {
                TypeName = dto.TypeName.Trim().ToLower(),
                RequiresBalance = dto.RequiresBalance
            };

            var created = await _requestTypeRepository.CreateRequestTypeAsync(requestType);

            return new RequestTypeResponseDto
            {
                Id = created.Id,
                TypeName = created.TypeName,
                RequiresBalance = created.RequiresBalance
            };
        }

        public async Task<bool> DeleteRequestTypeAsync(int id)
        {
            var exists = await _requestTypeRepository.RequestTypeExistsAsync(id);
            if (!exists)
                throw new Exception($"Request type with ID {id} not found");

            // Note: This will fail if there are related records (Requests, SubTypes)
            // Consider adding a check before deletion
            return await _requestTypeRepository.DeleteRequestTypeAsync(id);
        }

        public async Task<List<RequestTypeResponseDto>> GetAllRequestTypesAsync()
        {
            var requestTypes = await _requestTypeRepository.GetAllRequestTypesAsync();

            return requestTypes.Select(rt => new RequestTypeResponseDto
            {
                Id = rt.Id,
                TypeName = rt.TypeName,
                RequiresBalance = rt.RequiresBalance,
                }).ToList();
        }

        public async Task<RequestTypeResponseDto?> GetRequestTypeByIdAsync(int id)
        {
            var requestType = await _requestTypeRepository.GetRequestTypeByIdAsync(id);
            if (requestType == null)
                throw new Exception($"Request type with ID {id} not found");

            return new RequestTypeResponseDto
            {
                Id = requestType.Id,
                TypeName = requestType.TypeName,
                RequiresBalance = requestType.RequiresBalance,
                
            };
        }

        public async Task<RequestTypeResponseDto?> UpdateRequestTypeAsync(int id, UpdateRequestTypeDto dto)
        {
            var exists = await _requestTypeRepository.RequestTypeExistsAsync(id);
            if (!exists)
                throw new Exception($"Request type with ID {id} not found");

            var nameExists = await _requestTypeRepository.RequestTypeNameExistsAsync(dto.TypeName, id);
            if (nameExists)
                throw new Exception($"Request type with name '{dto.TypeName}' already exists");

            var requestType = new RequestType
            {
                Id = id,
                TypeName = dto.TypeName.Trim().ToLower(),
                RequiresBalance = dto.RequiresBalance
            };

            var updated = await _requestTypeRepository.UpdateRequestTypeAsync(requestType);
            if (updated == null)
                return null;

            return new RequestTypeResponseDto
            {
                Id = updated.Id,
                TypeName = updated.TypeName,
                RequiresBalance = updated.RequiresBalance
            };
        }
    }
}
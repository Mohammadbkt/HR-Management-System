using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.RequestSubType;
using HRMS.api.entities;
using HRMS.api.Repositories;

namespace HRMS.api.Services
{
    public interface IRequestSubTypeService
    {
        Task<RequestSubTypeResponseDto> CreateRequestSubTypeAsync(CreateRequestSubTypeDto dto);
        Task<RequestSubTypeResponseDto?> UpdateRequestSubTypeAsync(int id, UpdateRequestSubTypeDto dto);
        Task<bool> DeleteRequestSubTypeAsync(int id);
        Task<RequestSubTypeResponseDto?> GetRequestSubTypeByIdAsync(int id);
        Task<List<RequestSubTypeResponseDto>> GetAllRequestSubTypesAsync();
        Task<List<RequestSubTypeResponseDto>> GetRequestSubTypesByTypeIdAsync(int requestTypeId);
    }

    public class RequestSubTypeService : IRequestSubTypeService
    {
        private readonly IRequestSubTypeRepository _requestSubTypeRepository;
        private readonly IRequestTypeRepository _requestTypeRepository;

        public RequestSubTypeService(
            IRequestSubTypeRepository requestSubTypeRepository,
            IRequestTypeRepository requestTypeRepository)
        {
            _requestSubTypeRepository = requestSubTypeRepository;
            _requestTypeRepository = requestTypeRepository;
        }

        public async Task<RequestSubTypeResponseDto> CreateRequestSubTypeAsync(CreateRequestSubTypeDto dto)
        {
            var requestTypeExists = await _requestTypeRepository.RequestTypeExistsAsync(dto.RequestTypeId);
            if (!requestTypeExists)
                throw new Exception($"Request type with ID {dto.RequestTypeId} not found");

            var nameExists = await _requestSubTypeRepository.RequestSubTypeNameExistsAsync(
                dto.SubTypeName, 
                dto.RequestTypeId);
            if (nameExists)
                throw new Exception($"Request subtype with name '{dto.SubTypeName}' already exists for this request type");

            var requestSubType = new RequestSubType
            {
                SubTypeName = dto.SubTypeName.Trim().ToLower(),
                RequestTypeId = dto.RequestTypeId
            };

            var created = await _requestSubTypeRepository.CreateRequestSubTypeAsync(requestSubType);

            var requestType = await _requestTypeRepository.GetRequestTypeByIdAsync(dto.RequestTypeId);

            return new RequestSubTypeResponseDto
            {
                Id = created.Id,
                SubTypeName = created.SubTypeName,
                RequestTypeId = created.RequestTypeId,
                RequestTypeName = requestType?.TypeName
            };
        }

        public async Task<bool> DeleteRequestSubTypeAsync(int id)
        {
            var exists = await _requestSubTypeRepository.RequestSubTypeExistsAsync(id);
            if (!exists)
                throw new Exception($"Request subtype with ID {id} not found");

            // Note: This will fail if there are related Requests
            return await _requestSubTypeRepository.DeleteRequestSubTypeAsync(id);
        }

        public async Task<List<RequestSubTypeResponseDto>> GetAllRequestSubTypesAsync()
        {
            var requestSubTypes = await _requestSubTypeRepository.GetAllRequestSubTypesAsync();

            return requestSubTypes.Select(rst => new RequestSubTypeResponseDto
            {
                Id = rst.Id,
                SubTypeName = rst.SubTypeName,
                RequestTypeId = rst.RequestTypeId,
                RequestTypeName = rst.RequestType?.TypeName
            }).ToList();
        }

        public async Task<RequestSubTypeResponseDto?> GetRequestSubTypeByIdAsync(int id)
        {
            var requestSubType = await _requestSubTypeRepository.GetRequestSubTypeByIdAsync(id);
            if (requestSubType == null)
                throw new Exception($"Request subtype with ID {id} not found");

            return new RequestSubTypeResponseDto
            {
                Id = requestSubType.Id,
                SubTypeName = requestSubType.SubTypeName,
                RequestTypeId = requestSubType.RequestTypeId,
                RequestTypeName = requestSubType.RequestType?.TypeName
            };
        }

        public async Task<List<RequestSubTypeResponseDto>> GetRequestSubTypesByTypeIdAsync(int requestTypeId)
        {
            var requestTypeExists = await _requestTypeRepository.RequestTypeExistsAsync(requestTypeId);
            if (!requestTypeExists)
                throw new Exception($"Request type with ID {requestTypeId} not found");

            var requestSubTypes = await _requestSubTypeRepository.GetRequestSubTypesByTypeIdAsync(requestTypeId);

            return requestSubTypes.Select(rst => new RequestSubTypeResponseDto
            {
                Id = rst.Id,
                SubTypeName = rst.SubTypeName,
                RequestTypeId = rst.RequestTypeId,
                RequestTypeName = rst.RequestType?.TypeName
            }).ToList();
        }

        public async Task<RequestSubTypeResponseDto?> UpdateRequestSubTypeAsync(int id, UpdateRequestSubTypeDto dto)
        {
            var exists = await _requestSubTypeRepository.RequestSubTypeExistsAsync(id);
            if (!exists)
                throw new Exception($"Request subtype with ID {id} not found");

            var requestTypeExists = await _requestTypeRepository.RequestTypeExistsAsync(dto.RequestTypeId);
            if (!requestTypeExists)
                throw new Exception($"Request type with ID {dto.RequestTypeId} not found");

            var nameExists = await _requestSubTypeRepository.RequestSubTypeNameExistsAsync(
                dto.SubTypeName, 
                dto.RequestTypeId, 
                id);
            if (nameExists)
                throw new Exception($"Request subtype with name '{dto.SubTypeName}' already exists for this request type");

            var requestSubType = new RequestSubType
            {
                Id = id,
                SubTypeName = dto.SubTypeName.Trim().ToLower(),
                RequestTypeId = dto.RequestTypeId
            };

            var updated = await _requestSubTypeRepository.UpdateRequestSubTypeAsync(requestSubType);
            if (updated == null)
                return null;

            var requestType = await _requestTypeRepository.GetRequestTypeByIdAsync(dto.RequestTypeId);

            return new RequestSubTypeResponseDto
            {
                Id = updated.Id,
                SubTypeName = updated.SubTypeName,
                RequestTypeId = updated.RequestTypeId,
                RequestTypeName = requestType?.TypeName
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Request;
using HRMS.api.entities;
using HRMS.api.Repositories;

namespace HRMS.api.Services
{
    public interface IRequestService
    {
        Task<ResponseDto> CreateRequestAsync(int employeeId, CreateRequestDto dto);
        Task<ResponseDto?> UpdateRequestAsync(int id, UpdateRequestDto dto);
        Task<ResponseDto?> ChangeRequestStatusAsync(int employeeId, int id, RequestStatus request);
        Task<List<ResponseDto>> GetAllRequestsAsync();
        Task<ResponseDto?> GetRequestByIdAsync(int id);
        Task<List<ResponseDto>> GetAllRequestByEmployeeIdAsync(int id);
        Task<bool> DeleteRequestByIdAsync(int id);
    }

    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRequestTypeRepository _requestTypeRepository;
        private readonly IRequestSubTypeRepository _requestSubTypeRepository;
        private readonly IRequestBalanceRepository  _requestBalanceRepository;

        public RequestService(
            IRequestRepository requestRepository, IEmployeeRepository employeeRepository, IRequestTypeRepository requestTypeRepository, IRequestSubTypeRepository requestSubTypeRepository, IRequestBalanceRepository  requestBalanceRepository)
        {
            _requestRepository = requestRepository;
            _employeeRepository = employeeRepository;
            _requestTypeRepository = requestTypeRepository;
            _requestSubTypeRepository = requestSubTypeRepository;
            _requestBalanceRepository = requestBalanceRepository;
        }

        public async Task<ResponseDto> CreateRequestAsync(int employeeId, CreateRequestDto dto)
        {
            var employeeExists = await _employeeRepository.EmployeeExistsAsync(employeeId);
            if (!employeeExists)
                throw new Exception($"Employee with ID {employeeId} does not exist");

            var requestType = await _requestTypeRepository.GetRequestTypeByIdAsync(dto.RequestTypeId);
            if (requestType == null)
                throw new Exception($"Request type with ID {dto.RequestTypeId} does not exist");

            var requestSubType = await _requestSubTypeRepository.GetRequestSubTypeByIdAsync(dto.RequestSubTypeId);
            if (requestSubType == null)
                throw new Exception($"Request subtype with ID {dto.RequestSubTypeId} does not exist");

            if (requestSubType.RequestTypeId != dto.RequestTypeId)
                throw new Exception("The selected subtype does not belong to the selected request type");


            if (requestType.TypeName.Equals("leave", StringComparison.OrdinalIgnoreCase))
            {
                var today = DateTime.UtcNow.Date;
                
                if (dto.FromDate.Date != today)
                    throw new Exception("Leave requests must be for today only");

                if (dto.ToDate.Date != today)
                    throw new Exception("Leave requests must be for today only");

                if (dto.FromDate.Date != dto.ToDate.Date)
                    throw new Exception("Leave requests must be for the same day");

                if (!dto.FromTime.HasValue || !dto.ToTime.HasValue)
                    throw new Exception("Start time and end time are required for leave requests");

                if (dto.ToTime.Value <= dto.FromTime.Value)
                    throw new Exception("End time must be after start time");
            }
            else
            {
                if (dto.FromTime.HasValue || dto.ToTime.HasValue)
                    throw new Exception("Time specification is not allowed for vacation requests. Only full days are permitted");

                if (dto.FromDate.Date < DateTime.UtcNow.Date)
                    throw new Exception("Vacation requests must be for future dates");

                if (dto.ToDate.Date < dto.FromDate.Date)
                    throw new Exception("End date must be on or after start date");
            }

            var hasOverlap = await _requestRepository.HasOverlappingRequestAsync(employeeId, dto.FromDate, dto.ToDate, null);
            if (hasOverlap)
                throw new Exception($"You already have a request that overlaps with the dates {dto.FromDate:yyyy-MM-dd} to {dto.ToDate:yyyy-MM-dd}");


            var isLeave = requestType.TypeName.Equals("leave", StringComparison.OrdinalIgnoreCase);

            var requestedDays = CalculateRequestedDays(dto.FromDate, dto.ToDate, dto.FromTime, dto.ToTime, isLeave);

            if (requestType.RequiresBalance)
            {
                // TODO: Implement when RequestBalance is ready
                var balance = await _requestBalanceRepository.GetBalanceAsync(
                    employeeId, 
                    dto.RequestTypeId, 
                    dto.FromDate.Year);
                
                if (balance == null)
                    throw new Exception($"No balance found for {requestType.TypeName} in {dto.FromDate.Year}");
                
                if (balance.RemainingDays < requestedDays)
                    throw new Exception($"Insufficient balance. Available: {balance.RemainingDays} days, Requested: {requestedDays} days");
            }

            var request = new Request
            {
                EmployeeId = employeeId,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                FromTime = dto.FromTime,
                ToTime = dto.ToTime,
                Status = RequestStatus.pending,
                Notes = dto.Notes,
                RequestTypeId = dto.RequestTypeId,
                RequestSubTypeId = dto.RequestSubTypeId
            };

            var created = await _requestRepository.CreateRequestAsync(request);

            return new ResponseDto
            {
                Id = created.Id,
                EmployeeId = created.EmployeeId,
                FromDate = created.FromDate,
                ToDate = created.ToDate,
                FromTime = created.FromTime,
                ToTime = created.ToTime,
                Status = created.Status,
                Notes = created.Notes,
                RequestTypeId = created.RequestTypeId,
                RequestSubTypeId = created.RequestSubTypeId
            };
        }

        public async Task<ResponseDto?> UpdateRequestAsync(int id, UpdateRequestDto dto)
        {
            var existingRequest = await _requestRepository.GetRequestByIdAsync(id);
            if (existingRequest == null)
                throw new Exception($"Request with ID {id} not found");

            if (existingRequest.Status == RequestStatus.Approved || 
                existingRequest.Status == RequestStatus.Rejected)
            {
                throw new Exception("Cannot update approved or rejected requests");
            }

            var requestType = await _requestTypeRepository.GetRequestTypeByIdAsync(dto.RequestTypeId);
            if (requestType == null)
                throw new Exception($"Request type with ID {dto.RequestTypeId} does not exist");

            var requestSubType = await _requestSubTypeRepository.GetRequestSubTypeByIdAsync(dto.RequestSubTypeId);
            if (requestSubType == null)
                throw new Exception($"Request subtype with ID {dto.RequestSubTypeId} does not exist");

            if (requestSubType.RequestTypeId != dto.RequestTypeId)
                throw new Exception("The selected subtype does not belong to the selected request type");

            if (requestType.TypeName.Equals("leave", StringComparison.OrdinalIgnoreCase))
            {
                var today = DateTime.UtcNow.Date;
                
                if (dto.FromDate.Date != today)
                    throw new Exception("Leave requests must be for today only");

                if (dto.ToDate.Date != today)
                    throw new Exception("Leave requests must be for today only");

                if (dto.FromDate.Date != dto.ToDate.Date)
                    throw new Exception("Leave requests must be for the same day");

                if (!dto.FromTime.HasValue || !dto.ToTime.HasValue)
                    throw new Exception("Start time and end time are required for leave requests");

                if (dto.ToTime.Value <= dto.FromTime.Value)
                    throw new Exception("End time must be after start time");
            }
            else
            {
                if (dto.FromTime.HasValue || dto.ToTime.HasValue)
                    throw new Exception("Time specification is not allowed for vacation requests. Only full days are permitted");

                if (dto.FromDate.Date < DateTime.UtcNow.Date)
                    throw new Exception("Vacation requests must be for future dates");

                if (dto.ToDate.Date < dto.FromDate.Date)
                    throw new Exception("End date must be on or after start date");
            }

            var hasOverlap = await _requestRepository.HasOverlappingRequestAsync(
                existingRequest.EmployeeId, 
                dto.FromDate, 
                dto.ToDate, 
                id);
            if (hasOverlap)
                throw new Exception($"You already have a request that overlaps with the dates {dto.FromDate:yyyy-MM-dd} to {dto.ToDate:yyyy-MM-dd}");

            existingRequest.FromDate = dto.FromDate;
            existingRequest.ToDate = dto.ToDate;
            existingRequest.FromTime = dto.FromTime;
            existingRequest.ToTime = dto.ToTime;
            existingRequest.Notes = dto.Notes;
            existingRequest.RequestTypeId = dto.RequestTypeId;
            existingRequest.RequestSubTypeId = dto.RequestSubTypeId;

            var updated = await _requestRepository.UpdateRequestAsync(id, existingRequest);
            if (updated == null)
                throw new Exception("Failed to update request");

            return new ResponseDto
            {
                Id = updated.Id,
                EmployeeId = updated.EmployeeId,
                FromDate = updated.FromDate,
                ToDate = updated.ToDate,
                FromTime = updated.FromTime,
                ToTime = updated.ToTime,
                Status = updated.Status,
                Notes = updated.Notes,
                RequestTypeId = updated.RequestTypeId,
                RequestSubTypeId = updated.RequestSubTypeId
            };
        }

        public async Task<ResponseDto?> ChangeRequestStatusAsync(int employeeId, int id, RequestStatus newStatus)
        {
            var employeeExists = await _employeeRepository.EmployeeExistsAsync(employeeId);
            if (!employeeExists)
                throw new Exception($"Employee with ID {employeeId} does not exist");

            var existingRequest = await _requestRepository.GetRequestByIdAsync(id);
            if (existingRequest == null)
                throw new Exception($"Request with ID {id} not found");

            var oldStatus = existingRequest.Status;

        
            var updated = await _requestRepository.ChangeRequestStatusAsync(id, newStatus);
            if (updated == null)
                throw new Exception("Failed to update request status");

            // 6. Handle balance updates
            var requestType = await _requestTypeRepository.GetRequestTypeByIdAsync(updated.RequestTypeId);
            
            if (requestType != null && requestType.RequiresBalance)
            {
                var isLeave = requestType.TypeName.Equals("leave", StringComparison.OrdinalIgnoreCase);
                var requestedDays = CalculateRequestedDays(
                    updated.FromDate, 
                    updated.ToDate, 
                    updated.FromTime, 
                    updated.ToTime,
                    isLeave);
                var year = updated.FromDate.Year;

                // If approved, deduct from balance
                if (newStatus == RequestStatus.Approved && oldStatus == RequestStatus.pending)
                {
                    // TODO: Deduct balance
                    var deducted = await _requestBalanceRepository.DeductBalanceAsync(employeeId, requestType.Id, updated.FromDate.Year, requestedDays);
                    if(!deducted)
                        throw new Exception("error happen while deductying the balance");
                }

                // If cancelled/rejected after approval, restore balance
                if ((newStatus == RequestStatus.Cancelled || newStatus == RequestStatus.Rejected) 
                    && oldStatus == RequestStatus.Approved)
                {
                    // TODO: Restore balance
                    var deducted = await _requestBalanceRepository.RestoreBalanceAsync(employeeId, requestType.Id, updated.FromDate.Year, requestedDays);
                    if(!deducted)
                       throw new Exception("error happen while deductying the balance");
               
                }
            }

            return new ResponseDto
            {
                Id = updated.Id,
                EmployeeId = updated.EmployeeId,
                FromDate = updated.FromDate,
                ToDate = updated.ToDate,
                FromTime = updated.FromTime,
                ToTime = updated.ToTime,
                Status = updated.Status,
                Notes = updated.Notes,
                RequestTypeId = updated.RequestTypeId,
                RequestSubTypeId = updated.RequestSubTypeId
            };
        }

        public async Task<bool> DeleteRequestByIdAsync(int id)
        {
            var existingRequest = await _requestRepository.ISRequestExistAsync(id);
            if (!existingRequest)
                throw new Exception($"Request with ID {id} not found");

            // build soft deletion

            return await _requestRepository.DeleteRequestByIdAsync(id);
        }

        public async Task<List<ResponseDto>> GetAllRequestsAsync()
        {
            var requests = await _requestRepository.GetAllRequestsAsync();

            return requests.Select(r => new ResponseDto
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                FromDate = r.FromDate,
                ToDate = r.ToDate,
                FromTime = r.FromTime,
                ToTime = r.ToTime,
                Status = r.Status,
                Notes = r.Notes,
                RequestTypeId = r.RequestTypeId,
                RequestSubTypeId = r.RequestSubTypeId
            }).ToList();
        }

        public async Task<List<ResponseDto>> GetAllRequestByEmployeeIdAsync(int employeeId)
        {
            var employeeExists = await _employeeRepository.EmployeeExistsAsync(employeeId);
            if (!employeeExists)
                throw new Exception($"Employee with ID {employeeId} does not exist");

            var requests = await _requestRepository.GetAllRequestByEmployeeIdAsync(employeeId);

            return requests.Select(r => new ResponseDto
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                FromDate = r.FromDate,
                ToDate = r.ToDate,
                FromTime = r.FromTime,
                ToTime = r.ToTime,
                Status = r.Status,
                Notes = r.Notes,
                RequestTypeId = r.RequestTypeId,
                RequestSubTypeId = r.RequestSubTypeId
            }).ToList();
        }

        public async Task<ResponseDto?> GetRequestByIdAsync(int id)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request == null)
                throw new Exception($"Request with ID {id} not found");

            return new ResponseDto
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                FromTime = request.FromTime,
                ToTime = request.ToTime,
                Status = request.Status,
                Notes = request.Notes,
                RequestTypeId = request.RequestTypeId,
                RequestSubTypeId = request.RequestSubTypeId
            };
        }

        private decimal CalculateRequestedDays(DateTime fromDate, DateTime toDate, TimeSpan? fromTime, TimeSpan? toTime, bool isLeave)
        {
            if (isLeave && fromTime.HasValue && toTime.HasValue)
            {
                var duration = toTime.Value - fromTime.Value;
                var hours = duration.TotalHours;
                
                if (hours <= 4)
                    return 0.5m; 
                else
                    return 1.0m;
            }
            
            return (toDate.Date - fromDate.Date).Days + 1;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IRequestRepository
    {
        Task<Request> CreateRequestAsync(Request request);
        Task<Request?> UpdateRequestAsync(int id, Request request);
        Task<Request?> ChangeRequestStatusAsync(int id, RequestStatus request);
        Task<List<Request>> GetAllRequestsAsync();
        Task<Request?> GetRequestByIdAsync(int id);
        Task<List<Request>> GetAllRequestByEmployeeIdAsync(int id);
        Task<bool> DeleteRequestByIdAsync(int id);
        Task<bool> ISRequestExistAsync(int id);
        Task<bool> HasOverlappingRequestAsync(int employeeId, DateTime fromDate, DateTime toDate, int? excludeRequestId = null);
    }


    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _context;

        public RequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Request?> ChangeRequestStatusAsync(int id, RequestStatus requestStatus)
        {
            
            var existingRequest = await _context.Requests.FirstOrDefaultAsync(r=>r.Id == id);
            if(existingRequest == null)
                return null;
            
            existingRequest.Status = requestStatus;
            await _context.SaveChangesAsync();
            return existingRequest;
        }

        public async Task<Request> CreateRequestAsync(Request request)
        {
            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteRequestByIdAsync(int id)
        {
            var requestToDelete = await _context.Requests.FirstOrDefaultAsync(r=>r.Id == id);
            if(requestToDelete == null)
                return false;

            _context.Remove(requestToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Request>> GetAllRequestsAsync()
        {
            return await _context.Requests
                .Include(r=>r.Employee)
                    .ThenInclude(e=>e.User)
                .Include(r=>r.RequestType)
                .Include(r=>r.RequestSubType)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Request?> GetRequestByIdAsync(int id)
        {
            return await _context.Requests
                .Include(r=>r.Employee)
                    .ThenInclude(e=>e.User)
                .Include(r=>r.RequestType)
                .Include(r=>r.RequestSubType)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(r=>r.Id == id);
                
        }

        public async Task<bool> ISRequestExistAsync(int id)
        {
            return await _context.Requests.AnyAsync(r=>r.Id == id);
        }

        public async Task<Request?> UpdateRequestAsync(int id, Request request)
        {
            var existingRequest = await _context.Requests.FirstOrDefaultAsync(r=>r.Id == id);
            if(existingRequest == null)
                return null;
            
            existingRequest.EmployeeId = request.EmployeeId;
            existingRequest.FromDate = request.FromDate;
            existingRequest.ToDate = request.ToDate;
            existingRequest.FromTime = request.FromTime;
            existingRequest.ToTime = request.ToTime;
            existingRequest.Status = request.Status;
            existingRequest.Notes = request.Notes;
            existingRequest.RequestTypeId = request.RequestTypeId;
            existingRequest.RequestSubTypeId = request.RequestSubTypeId;

            await _context.SaveChangesAsync();
            return existingRequest;
        }

        public async Task<List<Request>> GetAllRequestByEmployeeIdAsync(int id)
        {
            return await _context.Requests
                .Include(r => r.Employee)
                    .ThenInclude(e => e.User)
                .Include(r => r.RequestType)
                .Include(r => r.RequestSubType)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(r => r.EmployeeId == id)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingRequestAsync(int employeeId, DateTime fromDate, DateTime toDate, int? excludeRequestId = null)
        {
            var query = _context.Requests.Where(r=>r.EmployeeId == employeeId 
                                            &&  r.Status != RequestStatus.Rejected &&
                                                r.Status != RequestStatus.Cancelled &&
                                                r.FromDate.Date <= toDate.Date &&
                                                r.ToDate.Date >= fromDate.Date
                                                );
                if (excludeRequestId.HasValue)
                    query = query.Where(r => r.Id != excludeRequestId.Value);

                return await query.AnyAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IRequestTypeRepository
    {
        Task<RequestType> CreateRequestTypeAsync(RequestType requestType);
        Task<RequestType?> GetRequestTypeByIdAsync(int id);
        Task<List<RequestType>> GetAllRequestTypesAsync();
        Task<RequestType?> UpdateRequestTypeAsync(RequestType requestType);
        Task<bool> DeleteRequestTypeAsync(int id);
        Task<bool> RequestTypeExistsAsync(int id);
        Task<bool> RequestTypeNameExistsAsync(string typeName, int? excludeId = null);
    }

    public class RequestTypeRepository : IRequestTypeRepository
    {
        private readonly AppDbContext _context;

        public RequestTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RequestType> CreateRequestTypeAsync(RequestType requestType)
        {
            await _context.RequestTypes.AddAsync(requestType);
            await _context.SaveChangesAsync();
            return requestType;
        }

        public async Task<bool> DeleteRequestTypeAsync(int id)
        {
            var requestType = await _context.RequestTypes.FindAsync(id);
            if (requestType == null)
                return false;

            _context.RequestTypes.Remove(requestType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RequestType>> GetAllRequestTypesAsync()
        {
            return await _context.RequestTypes
                .Include(rt => rt.SubTypes)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<RequestType?> GetRequestTypeByIdAsync(int id)
        {
            return await _context.RequestTypes
                .Include(rt => rt.SubTypes)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(rt => rt.Id == id);
        }

        public async Task<bool> RequestTypeExistsAsync(int id)
        {
            return await _context.RequestTypes.AnyAsync(rt => rt.Id == id);
        }

        public async Task<bool> RequestTypeNameExistsAsync(string typeName, int? excludeId = null)
        {
            var query = _context.RequestTypes.Where(rt => rt.TypeName.ToLower() == typeName.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(rt => rt.Id != excludeId.Value);
            
            return await query.AnyAsync();
        }

        public async Task<RequestType?> UpdateRequestTypeAsync(RequestType requestType)
        {
            var existingRequestType = await _context.RequestTypes.FindAsync(requestType.Id);
            if (existingRequestType == null)
                return null;

            existingRequestType.TypeName = requestType.TypeName;
            existingRequestType.RequiresBalance = requestType.RequiresBalance;

            await _context.SaveChangesAsync();
            return existingRequestType;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IRequestSubTypeRepository
    {
        Task<RequestSubType> CreateRequestSubTypeAsync(RequestSubType requestSubType);
        Task<RequestSubType?> GetRequestSubTypeByIdAsync(int id);
        Task<List<RequestSubType>> GetAllRequestSubTypesAsync();
        Task<List<RequestSubType>> GetRequestSubTypesByTypeIdAsync(int requestTypeId);
        Task<RequestSubType?> UpdateRequestSubTypeAsync(RequestSubType requestSubType);
        Task<bool> DeleteRequestSubTypeAsync(int id);
        Task<bool> RequestSubTypeExistsAsync(int id);
        Task<bool> RequestSubTypeNameExistsAsync(string subTypeName, int requestTypeId, int? excludeId = null);
    }

    public class RequestSubTypeRepository : IRequestSubTypeRepository
    {
        private readonly AppDbContext _context;

        public RequestSubTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RequestSubType> CreateRequestSubTypeAsync(RequestSubType requestSubType)
        {
            await _context.RequestSubTypes.AddAsync(requestSubType);
            await _context.SaveChangesAsync();
            return requestSubType;
        }

        public async Task<bool> DeleteRequestSubTypeAsync(int id)
        {
            var requestSubType = await _context.RequestSubTypes.FindAsync(id);
            if (requestSubType == null)
                return false;

            _context.RequestSubTypes.Remove(requestSubType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RequestSubType>> GetAllRequestSubTypesAsync()
        {
            return await _context.RequestSubTypes
                .Include(rst => rst.RequestType)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<RequestSubType?> GetRequestSubTypeByIdAsync(int id)
        {
            return await _context.RequestSubTypes
                .Include(rst => rst.RequestType)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(rst => rst.Id == id);
        }

        public async Task<List<RequestSubType>> GetRequestSubTypesByTypeIdAsync(int requestTypeId)
        {
            return await _context.RequestSubTypes
                .Include(rst => rst.RequestType)
                .Where(rst => rst.RequestTypeId == requestTypeId)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<bool> RequestSubTypeExistsAsync(int id)
        {
            return await _context.RequestSubTypes.AnyAsync(rst => rst.Id == id);
        }

        public async Task<bool> RequestSubTypeNameExistsAsync(string subTypeName, int requestTypeId, int? excludeId = null)
        {
            var query = _context.RequestSubTypes.Where(rst => 
                rst.SubTypeName.ToLower() == subTypeName.ToLower() && 
                rst.RequestTypeId == requestTypeId);
            
            if (excludeId.HasValue)
                query = query.Where(rst => rst.Id != excludeId.Value);
            
            return await query.AnyAsync();
        }

        public async Task<RequestSubType?> UpdateRequestSubTypeAsync(RequestSubType requestSubType)
        {
            var existingRequestSubType = await _context.RequestSubTypes.FindAsync(requestSubType.Id);
            if (existingRequestSubType == null)
                return null;

            existingRequestSubType.SubTypeName = requestSubType.SubTypeName;
            existingRequestSubType.RequestTypeId = requestSubType.RequestTypeId;

            await _context.SaveChangesAsync();
            return existingRequestSubType;
        }
    }
}
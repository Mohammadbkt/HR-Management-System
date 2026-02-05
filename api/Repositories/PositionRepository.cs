using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.Dtos.Position;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IPositionRepository
    {
        Task<Position> CreatePositionAsync(Position position);
        Task<Position?> UpdatePositionAsync(Position dto);
        Task<bool> DeletePositionAsync(Position position);
        Task<Position?> GetByIdAsync(int id);
        Task<List<Position>> GetAllAsync();
        Task<List<Position>> GetByDepartmentAsync(int departmentId);
        Task<Position?> GetByTitleAsync(string title, int? departmentId);
    
    }
    public class PositionRepository : IPositionRepository
    {
        private readonly AppDbContext _context;

        public PositionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Position> CreatePositionAsync(Position position)
        {
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();
            return position;
        }

        public async Task<bool> DeletePositionAsync(Position position)
        {
            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Position>> GetAllAsync()
        {
            return await _context.Positions.ToListAsync();
            
        }

        public async Task<List<Position>> GetByDepartmentAsync(int departmentId)
        {
            var positions = await _context.Positions.Where(p=>p.DepartmentId == departmentId).ToListAsync();
            
            return positions;
        }

        public async Task<Position?> GetByIdAsync(int id)
        {
            return await _context.Positions.Where(p=>p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Position?> GetByTitleAsync(string title, int? departmentId)
        {
            return await _context.Positions.Where(p=>p.Title == title && p.DepartmentId == departmentId).FirstOrDefaultAsync();
        }

        public async Task<Position?> UpdatePositionAsync(Position position)
        {
            var positionToUpdate = await _context.Positions.Where(p=>p.Id == position.Id).FirstOrDefaultAsync();

            if(positionToUpdate == null)
                return null;

            positionToUpdate!.Title = position.Title;
            positionToUpdate.Description = position.Description;
            positionToUpdate.DefaultBaseSalary = position.DefaultBaseSalary;
            positionToUpdate.IsActive = position.IsActive;
            positionToUpdate.DepartmentId = position.DepartmentId;

            
            await _context.SaveChangesAsync();

            return positionToUpdate;


            
        }
    }
}
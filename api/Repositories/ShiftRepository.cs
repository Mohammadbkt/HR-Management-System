using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.Dtos.Shift;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IShiftRepository
    {
        Task<Shift> CreateShiftAsync(Shift dto);
        Task<Shift?> GetShiftByIdAsync(int id);
        Task<List<Shift>> GetAllShiftsAsync();
        Task<Shift?> UpdateShiftAsync(Shift dto);
        Task<bool> DeleteShiftAsync(int id);
        Task<bool> IsShiftExist(int? id, TimeSpan start, TimeSpan end);


    }
    public class ShiftRepository : IShiftRepository
    {
        private readonly AppDbContext _context;

        public ShiftRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Shift> CreateShiftAsync(Shift shift)
        {
            if (shift.EndTime <= shift.StartTime)
                throw new ArgumentException("End time must be greater than start time.");


            await _context.Shifts.AddAsync(shift);
            await _context.SaveChangesAsync();
            return shift;
        }

        public async Task<bool> DeleteShiftAsync(int id)
        {
            var shiftToDelete = await GetShiftByIdAsync(id);

            if(shiftToDelete == null)
                return false;
            _context.Shifts.Remove(shiftToDelete);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Shift>> GetAllShiftsAsync()
        {
            return await _context.Shifts.ToListAsync();
        }

        public async Task<Shift?> GetShiftByIdAsync(int id)
        {
            return await _context.Shifts.FindAsync(id);
        }

        public async Task<bool> IsShiftExist(int? id, TimeSpan start, TimeSpan end)
        {
            var shift = _context.Shifts.Where(s => s.StartTime == start && s.EndTime == end);

            if (id.HasValue)
                shift = shift.Where(s => s.Id != id.Value);

            return await shift.AnyAsync();
        }

        public async Task<Shift?> UpdateShiftAsync(Shift dto)
        {
            var shiftToUpdate =  await _context.Shifts.Where(s=>s.Id == dto.Id).FirstOrDefaultAsync();
            if(shiftToUpdate == null)
                return null;
            
            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("End time must be greater than start time.");
        

            shiftToUpdate.StartTime = dto.StartTime;
            shiftToUpdate.EndTime = dto.EndTime;
            shiftToUpdate.RequiredHours = dto.RequiredHours;

            await _context.SaveChangesAsync();

            return shiftToUpdate;
        }
    }
}
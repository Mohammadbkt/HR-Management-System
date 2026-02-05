using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Data;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Repositories
{
    public interface IScheduleRepository
    {
        Task<Schedule?> GetScheduleByIdAsync(int employeeId, int shiftId, DateTime shiftDate);
        Task<Schedule> CreateScheduleAsync(Schedule schedule);
        Task<List<Schedule>> GetAllSchedulesAsync();
        Task<List<Schedule>> GetAllSchedulesByDepartmentAsync(int departmentId);
        Task<List<Schedule>> GetSchedulesByEmployeeIdAsync(int id);
        Task<bool> HasOverlappingShiftAsync(int employeeId, DateTime date, TimeSpan newShiftStart,  TimeSpan newShiftEnd);
        Task<bool> DeleteSchedulesAsync(int employeeId, DateTime shiftDate, int shiftId);
        Task<bool> ExistsAsync(int employeeId, int shiftId, DateTime date);
        // GetSchedulesByDateRangeAsync()
        // GetSchedulesByShiftIdAsync()
        // BulkCreateSchedulesAsync()

    }
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;

        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<bool> DeleteSchedulesAsync(int employeeId, DateTime shiftDate, int shiftId)
        {
            var schedule = await _context.Schedules.Where(s=>s.EmployeeId == employeeId && s.ShiftId == shiftId && s.ShiftDate == shiftDate).FirstOrDefaultAsync();
            if(schedule == null)
                return false;
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> ExistsAsync(int employeeId, int shiftId, DateTime shiftDate)
        {
            var schedule = await _context.Schedules.Where(s=>s.EmployeeId == employeeId && s.ShiftId == shiftId && s.ShiftDate == shiftDate).AnyAsync();
            
            return schedule;
        }

        public async Task<List<Schedule>> GetAllSchedulesAsync()
        {
                return await _context.Schedules
                    .Include(s => s.Employee).ThenInclude(e => e.User)
                    .Include(s => s.Shift)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .ToListAsync();
            
        }

        public async Task<List<Schedule>> GetAllSchedulesByDepartmentAsync(int departmentId)
        {
            var schedules = await _context.Schedules
                        .Include(s=>s.Employee)
                            .ThenInclude(e=>e.User)
                        .Include(s=>s.Shift)
                        .AsNoTracking()
                        .Where(s=>s.Employee.DepartmentId == departmentId)
                        .AsSplitQuery()
                        .ToListAsync();
                        
            return schedules;
        }

        public async Task<Schedule?> GetScheduleByIdAsync(int employeeId, int shiftId, DateTime shiftDate)
        {
            return await _context.Schedules
            .Include(s => s.Employee).ThenInclude(e => e.User)
            .Include(s => s.Shift)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.ShiftId == shiftId && s.ShiftDate == shiftDate);
        }

        public async Task<List<Schedule>> GetSchedulesByEmployeeIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Employee).ThenInclude(e => e.User)
                .Include(s => s.Shift)
                .Where(s => s.EmployeeId == id)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingShiftAsync(int employeeId, DateTime date, TimeSpan newShiftStart, TimeSpan newShiftEnd)
        {
            var existingSchedules = await _context.Schedules
            .Include(s => s.Shift)
            .Where(s => s.EmployeeId == employeeId && s.ShiftDate.Date == date.Date)
            .ToListAsync();

            foreach (var existing in existingSchedules)
            {
                bool overlaps = newShiftStart < existing.Shift.EndTime && newShiftEnd > existing.Shift.StartTime;
                if (overlaps)
                    return true;
            }

            return false;

        }
    }
}
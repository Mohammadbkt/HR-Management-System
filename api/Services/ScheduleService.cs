using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Schedule;
using HRMS.api.entities;
using HRMS.api.Repositories;

namespace HRMS.api.Services
{
    public interface IScheduleService
    {
        Task<ScheduleResponseDto> CreateScheduleAsync(CreateScheduleRequestDto schedule);
        Task<List<ScheduleResponseDto>> GetAllSchedulesAsync();
        Task<List<ScheduleResponseDto>> GetAllSchedulesByDepartmentAsync(int departmentId);
        Task<List<ScheduleResponseDto>> GetSchedulesByEmployeeIdAsync(int id);
        Task<ScheduleResponseDto?> UpdateSchedulesAsync(int oldEmployeeId, int oldShiftId, DateTime oldShiftDate, UpdateScheduleRequestDto newSchedule);
        Task<bool> DeleteSchedulesAsync(int employeeId, DateTime shiftDate, int shiftId);
        Task<ScheduleResponseDto?> GetScheduleByIdAsync(int employeeId, int shiftId, DateTime shiftDate);

        
        
    }
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IShiftRepository _shiftRepository;

        public ScheduleService(IScheduleRepository scheduleRepository, IEmployeeRepository employeeRepository, IShiftRepository shiftRepository)
        {
            _scheduleRepository = scheduleRepository;
            _employeeRepository = employeeRepository;
            _shiftRepository = shiftRepository;
        }

        public async Task<ScheduleResponseDto> CreateScheduleAsync(CreateScheduleRequestDto schedule)
        {
            var IsEmployeeExist =  await _employeeRepository.EmployeeExistsAsync(schedule.EmployeeId);
            if(!IsEmployeeExist)
                throw new Exception($"Employee with ID {schedule.EmployeeId} not found");
    

            var shift =  await _shiftRepository.GetShiftByIdAsync(schedule.ShiftId);
            if(shift == null)
                throw new Exception($"Shift with ID {schedule.ShiftId} not found");

            
            var IsExist =  await _scheduleRepository.ExistsAsync(schedule.EmployeeId,schedule.ShiftId, schedule.ShiftDate);
            if(IsExist)
                throw new Exception($"There is already a schedule for {schedule.EmployeeId} and Shift: {schedule.ShiftId} in : {schedule.ShiftDate}");
            
            if(schedule.ShiftDate <= DateTime.UtcNow)
                throw new Exception("you must assign a future date");

            var overlaps = await _scheduleRepository.HasOverlappingShiftAsync(schedule.EmployeeId, schedule.ShiftDate, shift.StartTime, shift.EndTime);
            if(overlaps)
                throw new Exception("you must assign a consistant time");


            var scheduleToCreate = new Schedule
            {
                EmployeeId = schedule.EmployeeId,
                ShiftId = schedule.ShiftId,
                ShiftDate = schedule.ShiftDate,
            };

            await _scheduleRepository.CreateScheduleAsync(scheduleToCreate);

            return new ScheduleResponseDto
            {
                EmployeeId = schedule.EmployeeId,
                ShiftId = schedule.ShiftId,
                ShiftDate = schedule.ShiftDate,
            };

        }

        public async Task<bool> DeleteSchedulesAsync(int employeeId, DateTime shiftDate, int shiftId)
        {
            var IsExist =  await _scheduleRepository.ExistsAsync(employeeId, shiftId, shiftDate);
            if(!IsExist)
                throw new Exception($"There is no schedule for {employeeId} and Shift: {shiftId} in : {shiftDate}");
            
            var IsDeleted = await _scheduleRepository.DeleteSchedulesAsync(employeeId, shiftDate, shiftId);
            return IsDeleted;
            
        }

        public async Task<List<ScheduleResponseDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllSchedulesAsync();

            return schedules.Select(s=> new ScheduleResponseDto
            {
                EmployeeId = s.EmployeeId,
                ShiftId = s.ShiftId,
                ShiftDate = s.ShiftDate
            }).ToList();
        }

        public async Task<List<ScheduleResponseDto>> GetAllSchedulesByDepartmentAsync(int departmentId)
        {
            var schedules =  await _scheduleRepository.GetAllSchedulesByDepartmentAsync(departmentId);
            return schedules.Select(s=> new ScheduleResponseDto
            {
                EmployeeId = s.EmployeeId,
                ShiftId = s.ShiftId,
                ShiftDate = s.ShiftDate
            }).ToList();
        }

        public async Task<ScheduleResponseDto?> GetScheduleByIdAsync(int employeeId, int shiftId, DateTime shiftDate)
        {
            var schedule =  await _scheduleRepository.GetScheduleByIdAsync(employeeId,  shiftId, shiftDate);
            if(schedule == null)
                throw new Exception("there is no schedule ");
            
            return new ScheduleResponseDto
            {
                EmployeeId = schedule.EmployeeId,
                ShiftId = schedule.ShiftId,
                ShiftDate = schedule.ShiftDate,
            };
        }

        public async Task<List<ScheduleResponseDto>> GetSchedulesByEmployeeIdAsync(int id)
        {
            var schedules = await _scheduleRepository.GetSchedulesByEmployeeIdAsync(id);

            return schedules.Select(s=> new ScheduleResponseDto
            {
                EmployeeId = s.EmployeeId,
                ShiftId = s.ShiftId,
                ShiftDate = s.ShiftDate
            }).ToList();
        }

        
        // Service
    public async Task<ScheduleResponseDto?> UpdateSchedulesAsync(int oldEmployeeId, int oldShiftId, DateTime oldShiftDate,UpdateScheduleRequestDto newSchedule)
    {
        var existingSchedule = await _scheduleRepository.GetScheduleByIdAsync(oldEmployeeId, oldShiftId, oldShiftDate);
        if(existingSchedule == null)
            throw new Exception($"Schedule not found");


        if(newSchedule.EmployeeId != oldEmployeeId)
        {
            var employeeExists = await _employeeRepository.EmployeeExistsAsync(newSchedule.EmployeeId);
            if(!employeeExists)
                throw new Exception($"Employee with ID {newSchedule.EmployeeId} not found");
        }
        
        var shift = await _shiftRepository.GetShiftByIdAsync(newSchedule.ShiftId);
        if(shift == null)
            throw new Exception($"Shift with ID {newSchedule.ShiftId} not found");


        if(newSchedule.ShiftDate.Date <= DateTime.UtcNow.Date)
            throw new Exception("Schedule must be for a future date");
    
        
        if (newSchedule.EmployeeId != oldEmployeeId || newSchedule.ShiftId != oldShiftId || newSchedule.ShiftDate != oldShiftDate)
        {
            var newExists = await _scheduleRepository.ExistsAsync(
                newSchedule.EmployeeId, newSchedule.ShiftId, newSchedule.ShiftDate);
            if(newExists)
                throw new Exception("A schedule already exists for this employee, shift, and date");

        }

        
        if (newSchedule.EmployeeId != oldEmployeeId || newSchedule.ShiftId != oldShiftId || newSchedule.ShiftDate.Date != oldShiftDate.Date)
        {
            var overlaps = await _scheduleRepository.HasOverlappingShiftAsync(
                newSchedule.EmployeeId, newSchedule.ShiftDate, shift.StartTime, shift.EndTime);
            if(overlaps)
                throw new Exception($"Employee already has an overlapping shift on {newSchedule.ShiftDate:yyyy-MM-dd}");
        }
        
        
        await _scheduleRepository.DeleteSchedulesAsync(oldEmployeeId, oldShiftDate, oldShiftId);
        
        var newScheduleEntity = new Schedule
        {
            EmployeeId = newSchedule.EmployeeId,
            ShiftId = newSchedule.ShiftId,
            ShiftDate = newSchedule.ShiftDate
        };
        
        await _scheduleRepository.CreateScheduleAsync(newScheduleEntity);
        
        return new ScheduleResponseDto
        {
            EmployeeId = newSchedule.EmployeeId,
            ShiftId = newSchedule.ShiftId,
            ShiftDate = newSchedule.ShiftDate
        };
    }

    }
}
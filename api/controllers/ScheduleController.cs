using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Schedule;
using HRMS.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controller
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleResponseDto>> CreateSchedule(
            [FromBody] CreateScheduleRequestDto request)
        {
            try
            {
                var schedule = await _scheduleService.CreateScheduleAsync(request);
                return CreatedAtAction(
                    nameof(GetScheduleById), 
                    new { employeeId = schedule.EmployeeId, shiftId = schedule.ShiftId, shiftDate = schedule.ShiftDate }, 
                    schedule);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<ScheduleResponseDto>>> GetAllSchedules()
        {
            try
            {
                var schedules = await _scheduleService.GetAllSchedulesAsync();
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<List<ScheduleResponseDto>>> GetSchedulesByDepartment(int departmentId)
        {
            try
            {
                var schedules = await _scheduleService.GetAllSchedulesByDepartmentAsync(departmentId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<List<ScheduleResponseDto>>> GetSchedulesByEmployee(int employeeId)
        {
            try
            {
                var schedules = await _scheduleService.GetSchedulesByEmployeeIdAsync(employeeId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{employeeId}/{shiftId}/{shiftDate}")]
        public async Task<ActionResult<ScheduleResponseDto>> GetScheduleById(
            int employeeId, 
            int shiftId, 
            DateTime shiftDate)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(employeeId, shiftId, shiftDate);
                if (schedule == null)
                    return NotFound(new { message = "Schedule not found" });
                
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{employeeId}/{shiftId}/{shiftDate}")]
        public async Task<ActionResult<ScheduleResponseDto>> UpdateSchedule(
            int employeeId, 
            int shiftId, 
            DateTime shiftDate,
            [FromBody] UpdateScheduleRequestDto request)
        {
            try
            {
                var updatedSchedule = await _scheduleService.UpdateSchedulesAsync(
                    employeeId, shiftId, shiftDate, request);
                
                if (updatedSchedule == null)
                    return NotFound(new { message = "Schedule not found" });
                
                return Ok(updatedSchedule);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{employeeId}/{shiftId}/{shiftDate}")]
        public async Task<ActionResult> DeleteSchedule(
            int employeeId, 
            int shiftId, 
            DateTime shiftDate)
        {
            try
            {
                var isDeleted = await _scheduleService.DeleteSchedulesAsync(employeeId, shiftDate, shiftId);
                
                if (!isDeleted)
                    return NotFound(new { message = "Schedule not found" });
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Shift;
using HRMS.api.Repositories;
using HRMS.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controller
{
    [ApiController]
    [Route("api/shift")]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShifts()
        {
            try
            {
                var shifts = await _shiftService.GetAllShiftsAsync();
                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShiftById(int id)
        {
            try
            {
                var shift = await _shiftService.GetShiftByIdAsync(id);
                return Ok(shift);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateShift([FromBody] CreateShiftRequestDto dto)
        {
            try
            {
                var shift = await _shiftService.CreateShiftAsync(dto);
                return CreatedAtAction(nameof(GetShiftById), new { id = shift.Id }, shift);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShift(int id, [FromBody] UpdateShiftRequestDto dto)
        {
            try
            {
                var shift = await _shiftService.UpdateShiftAsync(id, dto);
                return Ok(shift);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShift(int id)
        {
            try
            {
                var result = await _shiftService.DeleteShiftAsync(id);
                return Ok(new { message = "Shift deleted successfully", deleted = result });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
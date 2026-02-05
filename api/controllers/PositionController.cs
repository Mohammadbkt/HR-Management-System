using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Position;
using HRMS.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controller
{
    [ApiController]
    [Route("api/position")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpPost]
        public async Task<ActionResult<PositionResponseDto>> CreatePosition([FromBody] CreatePositionDto dto)
        {
            try
            {
                var position = await _positionService.CreatePositionAsync(dto);
                return CreatedAtAction(nameof(GetPositionById), new { id = position.Id }, position);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<PositionResponseDto>>> GetAllPositions()
        {
            try
            {
                var positions = await _positionService.GetAllPositionsAsync();
                return Ok(positions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PositionResponseDto>> GetPositionById(int id)
        {
            try
            {
                var position = await _positionService.GetPositionByIdAsync(id);
                return Ok(position);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<List<PositionResponseDto>>> GetPositionsByDepartment(int departmentId)
        {
            try
            {
                var positions = await _positionService.GetPositionsByDepartmentAsync(departmentId);
                return Ok(positions);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PositionResponseDto>> UpdatePosition(int id, [FromBody] UpdatePositionDto dto)
        {
            try
            {
                var position = await _positionService.UpdatePositionAsync(id, dto);
                return Ok(position);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePosition(int id)
        {
            try
            {
                var result = await _positionService.DeletePositionAsync(id);
                return Ok(new { message = "Position deleted successfully", deleted = result });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("check-unique")]
        public async Task<ActionResult<bool>> CheckPositionTitleUnique([FromQuery] string title, [FromQuery] int? departmentId)
        {
            try
            {
                var isUnique = await _positionService.IsPositionTitleUniqueAsync(title, departmentId);
                return Ok(new { title, departmentId, isUnique });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
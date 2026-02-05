using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRMS.api.Dtos.SalaryType;
using HRMS.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controllers
{
    [ApiController]
    [Route("api/salary-types")]
    public class SalaryTypeController : ControllerBase
    {
        private readonly ISalaryTypesService _salaryTypesService;

        public SalaryTypeController(ISalaryTypesService salaryTypesService)
        {
            _salaryTypesService = salaryTypesService;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateSalaryType([FromBody] CreateSalaryTypeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created =  await _salaryTypesService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetSalaryTypeById), new { id = created.Id }, created);

            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the salary type", error = ex.Message });
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalaryTypes()
        {
            try
            {
                var types = await _salaryTypesService.GetAllAsync();
                return Ok(types);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving salary types", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalaryTypeById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid salary type ID" });

                var type = await _salaryTypesService.GetByIdAsync(id);
                
                if (type == null)
                    return NotFound(new { message = $"Salary type with ID {id} not found" });

                return Ok(type);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the salary type", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalaryType(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid salary type ID" });


                var result = await _salaryTypesService.DeleteAsync(id);

                if (!result)
                    return NotFound(new { message = $"Salary type with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the salary type", error = ex.Message });
            }
        }
    }
}
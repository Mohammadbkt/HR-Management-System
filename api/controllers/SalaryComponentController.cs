using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.SalaryComponent;
using HRMS.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controllers
{
    [ApiController]
    [Route("api/salary-component")]
    [Authorize]
    public class SalaryComponentController : ControllerBase
    {
        private readonly ISalaryComponentService _salaryComponentService;

        public SalaryComponentController(ISalaryComponentService salaryComponentService)
        {
            _salaryComponentService = salaryComponentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ResponseDto>>> GetAll()
        {
            try
            {
                var components = await _salaryComponentService.GetAllAsync();
                return Ok(components);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving salary components", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            try
            {
                var component = await _salaryComponentService.GetByIdAsync(id);
                return Ok(component);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<List<ResponseDto>>> GetByEmployeeId(int employeeId)
        {
            try
            {
                var components = await _salaryComponentService.GetByEmployeeIdAsync(employeeId);
                return Ok(components);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving employee salary components", error = ex.Message });
            }
        }


        [HttpGet("employee/{employeeId}/breakdown")]
        public async Task<ActionResult<SalaryBreakdownDto>> GetDetailedSalary(int employeeId)
        {
            try
            {
                var breakdown = await _salaryComponentService.GetDetailedSalaryAsync(employeeId);
                return Ok(breakdown);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("breakdown/all")]
        public async Task<ActionResult<List<SalaryBreakdownDto>>> GetAllDetailedSalary()
        {
            try
            {
                var breakdowns = await _salaryComponentService.GetAllDetailedSalaryAsync();
                return Ok(breakdowns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving salary breakdowns", error = ex.Message });
            }
        }


        [HttpGet("employee/{employeeId}/allowances")]
        public async Task<ActionResult<decimal>> GetTotalAllowances(int employeeId)
        {
            try
            {
                var total = await _salaryComponentService.GetTotalAllowancesAsync(employeeId);
                return Ok(new { employeeId, totalAllowances = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating allowances", error = ex.Message });
            }
        }


        [HttpGet("employee/{employeeId}/deductions")]
        public async Task<ActionResult<decimal>> GetTotalDeductions(int employeeId)
        {
            try
            {
                var total = await _salaryComponentService.GetTotalDeductionsAsync(employeeId);
                return Ok(new { employeeId, totalDeductions = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating deductions", error = ex.Message });
            }
        }



        [HttpGet("employee/{employeeId}/net-salary")]
        public async Task<ActionResult<decimal>> GetNetSalary(int employeeId)
        {
            try
            {
                var netSalary = await _salaryComponentService.GetNetSalaryAsync(employeeId);
                return Ok(new { employeeId, netSalary });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating net salary", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateSalaryComponentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _salaryComponentService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating salary component", error = ex.Message });
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] UpdateSalaryComponentDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _salaryComponentService.UpdateAsync(id, updateDto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating salary component", error = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _salaryComponentService.DeleteAsync(id);
                if (result)
                    return NoContent();
                
                return NotFound(new { message = $"Salary component with ID {id} not found" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting salary component", error = ex.Message });
            }
        }
    }
}
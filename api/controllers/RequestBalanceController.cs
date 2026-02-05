using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRMS.api.Dtos.RequestBalance;
using HRMS.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controllers
{
    [ApiController]
    [Route("api/request-balances")]
    public class RequestBalanceController : ControllerBase
    {
        private readonly IRequestBalanceService _requestBalanceService;

        public RequestBalanceController(IRequestBalanceService requestBalanceService)
        {
            _requestBalanceService = requestBalanceService;
        }

        /// <summary>
        /// Create a new balance for an employee
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RequestBalanceResponseDto>> CreateBalance(
            [FromBody] CreateRequestBalanceDto request)
        {
            try
            {
                var balance = await _requestBalanceService.CreateBalanceAsync(request);
                return CreatedAtAction(
                    nameof(GetBalance),
                    new { employeeId = balance.EmployeeId, requestTypeId = balance.RequestTypeId, year = balance.Year },
                    balance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all balances (for admin/HR)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<RequestBalanceResponseDto>>> GetAllBalances()
        {
            try
            {
                var balances = await _requestBalanceService.GetAllBalancesAsync();
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific balance
        /// </summary>
        [HttpGet("{employeeId}/{requestTypeId}/{year}")]
        public async Task<ActionResult<RequestBalanceResponseDto>> GetBalance(
            int employeeId,
            int requestTypeId,
            int year)
        {
            try
            {
                var balance = await _requestBalanceService.GetBalanceAsync(employeeId, requestTypeId, year);
             
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all balances for a specific employee in a year
        /// </summary>
        [HttpGet("employee/{employeeId}/year/{year}")]
        public async Task<ActionResult<List<RequestBalanceResponseDto>>> GetBalancesByEmployee(
            int employeeId,
            int year)
        {
            try
            {
                var balances = await _requestBalanceService.GetAllBalanceByEmployeeIdAsync(employeeId, year);
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all balances for a specific year (for admin/HR)
        /// </summary>
        [HttpGet("year/{year}")]
        public async Task<ActionResult<List<RequestBalanceResponseDto>>> GetBalancesByYear(int year)
        {
            try
            {
                var balances = await _requestBalanceService.GetBalancesByYearAsync(year);
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update a balance
        /// </summary>
        [HttpPut("{employeeId}/{requestTypeId}/{year}")]
        public async Task<ActionResult<RequestBalanceResponseDto>> UpdateBalance([FromBody] UpdateRequestBalanceDto request)
        {
            try
            {
                var updatedBalance = await _requestBalanceService.UpdateBalanceAsync(request);

                if (updatedBalance == null)
                    return NotFound(new { message = "Balance not found" });

                return Ok(updatedBalance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a balance
        /// </summary>
        [HttpDelete("{employeeId}/{requestTypeId}/{year}")]
        public async Task<ActionResult> DeleteBalance(
            int employeeId,
            int requestTypeId,
            int year)
        {
            try
            {
                var isDeleted = await _requestBalanceService.DeleteBalanceAsync(employeeId, requestTypeId, year);

                if (!isDeleted)
                    return NotFound(new { message = "Balance not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Manually deduct days from balance (for admin adjustments)
        /// </summary>
        [HttpPost("{employeeId}/{requestTypeId}/{year}/deduct")]
        public async Task<ActionResult> DeductBalance(int employeeId, int requestTypeId, int year, int days)
        {
            try
            {
                var success = await _requestBalanceService.DeductBalanceAsync(employeeId, requestTypeId, year, days);

                if (!success)
                    return BadRequest(new { message = "Failed to deduct balance" });

                return Ok(new { message = $"Successfully deducted {days} days" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Manually restore days to balance (for admin adjustments)
        /// </summary>
        [HttpPost("{employeeId}/{requestTypeId}/{year}/restore")]
        public async Task<ActionResult> RestoreBalance(int employeeId, int requestTypeId, int year, int days)
        {
            try
            {
                var success = await _requestBalanceService.RestoreBalanceAsync(employeeId, requestTypeId, year, days);

                if (!success)
                    return BadRequest(new { message = "Failed to restore balance" });

                return Ok(new { message = $"Successfully restored {days} days" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRMS.api.Dtos.RequestType;
using HRMS.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controllers
{
    [ApiController]
    [Route("api/request-types")]
    public class RequestTypeController : ControllerBase
    {
        private readonly IRequestTypeService _requestTypeService;

        public RequestTypeController(IRequestTypeService requestTypeService)
        {
            _requestTypeService = requestTypeService;
        }

        [HttpPost]
        public async Task<ActionResult<RequestTypeResponseDto>> CreateRequestType(
            [FromBody] CreateRequestTypeDto request)
        {
            try
            {
                var requestType = await _requestTypeService.CreateRequestTypeAsync(request);
                return CreatedAtAction(
                    nameof(GetRequestTypeById),
                    new { id = requestType.Id },
                    requestType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<RequestTypeResponseDto>>> GetAllRequestTypes()
        {
            try
            {
                var requestTypes = await _requestTypeService.GetAllRequestTypesAsync();
                return Ok(requestTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequestTypeResponseDto>> GetRequestTypeById(int id)
        {
            try
            {
                var requestType = await _requestTypeService.GetRequestTypeByIdAsync(id);
                if (requestType == null)
                    return NotFound(new { message = "Request type not found" });

                return Ok(requestType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RequestTypeResponseDto>> UpdateRequestType(
            int id,
            [FromBody] UpdateRequestTypeDto request)
        {
            try
            {
                var updatedRequestType = await _requestTypeService.UpdateRequestTypeAsync(id, request);
                if (updatedRequestType == null)
                    return NotFound(new { message = "Request type not found" });

                return Ok(updatedRequestType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRequestType(int id)
        {
            try
            {
                var isDeleted = await _requestTypeService.DeleteRequestTypeAsync(id);
                if (!isDeleted)
                    return NotFound(new { message = "Request type not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRMS.api.Dtos.RequestSubType;
using HRMS.api.Repositories;
using HRMS.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controllers
{
    [ApiController]
    [Route("api/request-subtypes")]
    public class RequestSubTypeController : ControllerBase
    {
        private readonly IRequestSubTypeService _requestSubTypeService;

        public RequestSubTypeController(IRequestSubTypeService requestSubTypeService)
        {
            _requestSubTypeService = requestSubTypeService;
        }

        [HttpPost]
        public async Task<ActionResult<RequestSubTypeResponseDto>> CreateRequestSubType([FromBody] CreateRequestSubTypeDto request)
        {
            try
            {
                var requestSubType = await _requestSubTypeService.CreateRequestSubTypeAsync(request);
                return CreatedAtAction(
                    nameof(GetRequestSubTypeById),
                    new { id = requestSubType.Id },
                    requestSubType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<RequestSubTypeResponseDto>>> GetAllRequestSubTypes()
        {
            try
            {
                var requestSubTypes = await _requestSubTypeService.GetAllRequestSubTypesAsync();
                return Ok(requestSubTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequestSubTypeResponseDto>> GetRequestSubTypeById(int id)
        {
            try
            {
                var requestSubType = await _requestSubTypeService.GetRequestSubTypeByIdAsync(id);
                if (requestSubType == null)
                    return NotFound(new { message = "Request subtype not found" });

                return Ok(requestSubType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("by-type/{requestTypeId}")]
        public async Task<ActionResult<List<RequestSubTypeResponseDto>>> GetRequestSubTypesByTypeId(int requestTypeId)
        {
            try
            {
                var requestSubTypes = await _requestSubTypeService.GetRequestSubTypesByTypeIdAsync(requestTypeId);
                return Ok(requestSubTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RequestSubTypeResponseDto>> UpdateRequestSubType(
            int id,
            [FromBody] UpdateRequestSubTypeDto request)
        {
            try
            {
                var updatedRequestSubType = await _requestSubTypeService.UpdateRequestSubTypeAsync(id, request);
                if (updatedRequestSubType == null)
                    return NotFound(new { message = "Request subtype not found" });

                return Ok(updatedRequestSubType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRequestSubType(int id)
        {
            try
            {
                var isDeleted = await _requestSubTypeService.DeleteRequestSubTypeAsync(id);
                if (!isDeleted)
                    return NotFound(new { message = "Request subtype not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
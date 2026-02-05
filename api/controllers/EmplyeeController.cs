using HRMS.api.Dtos.common;
using HRMS.api.Dtos.Employee;
using HRMS.api.entities;
using HRMS.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/employees")]

public class EmployeesController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService, UserManager<User> userManager)
    {
        _employeeService = employeeService;
        _userManager = userManager;
    }

    [HttpPost("complete-profile")]
    public async Task<IActionResult> CompleteEmployeeProfile([FromBody] CreateEmployeeRequestDto dto)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!int.TryParse(userId, out int parsedUserId))
                return BadRequest("Invalid user ID format");

            var createdEmployee = await _employeeService.CreateEmployeeProfileAsync(parsedUserId, dto);
            if (createdEmployee == null)
                return BadRequest("Profile creation failed");

            return CreatedAtAction(nameof(GetMyEmployeeProfile), new { id = parsedUserId }, createdEmployee);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
        }
    }

    [HttpGet("my-profile")]
    public async Task<IActionResult> GetMyEmployeeProfile()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                return Unauthorized();

            var employee = await _employeeService.GetMyEmployeeProfileAsync(parsedUserId);
            if (employee == null)
                return NotFound("Employee profile not found");

            return Ok(employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving profile", detail = ex.Message });
        }
    }

    [HttpGet("{employeeId:int}")]
    public async Task<IActionResult> GetEmployeeById(int employeeId)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
                return NotFound("Employee profile not found");

            return Ok(employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving employee", detail = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<EmployeeResponseDto>>> GetAllEmployees([FromQuery] EmployeeQueryDto query)
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync(query);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error while retrieving employees", detail = ex.Message });
        }
    }

    [HttpDelete("my-profile")]
    public async Task<IActionResult> DeleteMyProfile()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                return Unauthorized();

            var isDeleted = await _employeeService.DeleteEmployeeProfileAsync(parsedUserId);
            if (!isDeleted)
                return StatusCode(500, "Error occurred while deleting the profile");

            return Ok(new { message = "Profile deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting profile", detail = ex.Message });
        }
    }
}

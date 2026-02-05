using System;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Department;
using HRMS.api.entities;
using HRMS.api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.api.Controllers
{
    [ApiController]
[Route("api/department")]

public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly UserManager<User> _userManager;

    public DepartmentsController(IDepartmentRepository departmentRepository, UserManager<User> userManager)
    {
        _departmentRepository = departmentRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDepartments()
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync();

            var depts = departments.Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                DepartmentName = d.Name,
                ManagerId = d.ManagerId,
                ManagerName = d.Manager?.User?.FullName
            }).ToList();

            return Ok(depts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving departments", detail = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return NotFound(new { message = $"Department with ID {id} not found" });

            return Ok(new DepartmentResponseDto
            {
                Id = department.Id,
                DepartmentName = department.Name,
                ManagerId = department.ManagerId,
                ManagerName = department.Manager?.User?.FullName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving department", detail = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(dto.ManagerId.HasValue)
            {
                var manager = await _userManager.FindByIdAsync(dto.ManagerId.Value.ToString());
                if (manager == null)
                    return BadRequest(new { message = $"Manager with ID {dto.ManagerId} does not exist" });
            }
            
            var department = new Department
            {
                Name = dto.DepartmentName,
                ManagerId = dto.ManagerId ?? null
            };

            var created = await _departmentRepository.CreateAsync(department);

            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new DepartmentResponseDto
                {
                    Id = created.Id,
                    DepartmentName = created.Name,
                    ManagerId = created.ManagerId ?? null
                });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating department", detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _departmentRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Department with ID {id} not found" });

            if(dto.ManagerId.HasValue)
            {
                var manager = await _userManager.FindByIdAsync(dto.ManagerId.Value.ToString());
                if (manager == null)
                    return BadRequest(new { message = $"Manager with ID {dto.ManagerId} does not exist" });
            }
            existing.Name = dto.DepartmentName;
            existing.ManagerId = dto.ManagerId ?? null;

            var updated = await _departmentRepository.UpdateAsync(id, existing);
            if(updated == null)
                return NotFound(new { message = $"Department with ID {id} not found for update" });

            return Ok(new DepartmentResponseDto
            {
                Id = updated.Id,
                DepartmentName = updated.Name,
                ManagerId = updated.ManagerId,
                ManagerName = updated.Manager?.User?.FullName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating department", detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _departmentRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Department with ID {id} not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting department", detail = ex.Message });
        }
    }
}

}
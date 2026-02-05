using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Position;
using HRMS.api.entities;
using HRMS.api.Repositories;

namespace HRMS.api.Services
{
    public interface IPositionService
    {
        Task<PositionResponseDto> CreatePositionAsync(CreatePositionDto dto);
        Task<PositionResponseDto?> UpdatePositionAsync(int id, UpdatePositionDto dto);
        Task<bool> DeletePositionAsync(int id);
        Task<PositionResponseDto?> GetPositionByIdAsync(int id);
        Task<List<PositionResponseDto>> GetAllPositionsAsync();
        Task<List<PositionResponseDto>> GetPositionsByDepartmentAsync(int departmentId);
        Task<bool> IsPositionTitleUniqueAsync(string title, int? departmentId);

        
    }

    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public PositionService(IPositionRepository positionRepository, IDepartmentRepository departmentRepository)
        {
            _positionRepository = positionRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<PositionResponseDto> CreatePositionAsync(CreatePositionDto dto)
        {
            var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
            if (department == null)
            {
                throw new Exception($"Department with ID {dto.DepartmentId} not found.");
            }
        

            var existingPostion = await _positionRepository.GetByTitleAsync( dto.Title, dto.DepartmentId);
            if(existingPostion != null)
            {
                throw new Exception($"Position with title '{dto.Title}' already exists in this department.");
            }

            

            if (dto.DefaultBaseSalary < 0)
            {
                throw new Exception("Base salary cannot be negative.");
            }

            var position = new Position()
            {
                Title = dto.Title,
                DefaultBaseSalary = dto.DefaultBaseSalary ?? 500,
                DepartmentId = dto.DepartmentId,
                IsActive = true,
                Description = dto.Description
            };


            var Createdposition = await _positionRepository.CreatePositionAsync(position);

            return new PositionResponseDto()
            {
                Id = Createdposition.Id,
                Title = Createdposition.Title,
                DefaultBaseSalary = Createdposition.DefaultBaseSalary,
                DepartmentId = Createdposition.DepartmentId,
                IsActive = true,
                Description = Createdposition.Description
            };
        }

        public async Task<bool> DeletePositionAsync(int id)
        {
            var existingPostion = await _positionRepository.GetByIdAsync(id);
            if(existingPostion == null)
            {
                throw new Exception($"Position with Id: '{id}' does not exists.");
            }

            var IsDeleted =  await _positionRepository.DeletePositionAsync(existingPostion!);
            return IsDeleted;
        }

        public async Task<List<PositionResponseDto>> GetAllPositionsAsync()
        {
            var positions = await _positionRepository.GetAllAsync();

            return positions.Select(p=>new PositionResponseDto()
            {
                Id = p.Id,
                Title = p.Title,
                DefaultBaseSalary = p.DefaultBaseSalary,
                DepartmentId = p.DepartmentId,
                IsActive = p.IsActive,
                Description = p.Description
            }).ToList();
        }

        public async Task<PositionResponseDto?> GetPositionByIdAsync(int id)
        {
            var position = await _positionRepository.GetByIdAsync(id);
            if(position == null)
            {
                throw new Exception($"There is no position with this Id: '{id}'.");
            }


            return new PositionResponseDto()
            {
                Id = position!.Id,
                Title = position.Title,
                DefaultBaseSalary = position.DefaultBaseSalary,
                DepartmentId = position.DepartmentId,
                IsActive = position.IsActive,
                Description = position.Description
            };
        }

        public async Task<List<PositionResponseDto>> GetPositionsByDepartmentAsync(int departmentId)
        {
            var positions = await _positionRepository.GetByDepartmentAsync(departmentId);
            if(positions == null || positions.Count == 0)
            {
                
                throw new Exception($"There is no position in Department {departmentId}.");
            }

            return positions.Select(p=>new PositionResponseDto()
            {
                Id = p.Id,
                Title = p.Title,
                DefaultBaseSalary = p.DefaultBaseSalary,
                DepartmentId = p.DepartmentId,
                IsActive = p.IsActive,
                Description = p.Description
            }).ToList();
        }

        public async Task<bool> IsPositionTitleUniqueAsync(string title, int? departmentId)
        {
            var position =  await _positionRepository.GetByTitleAsync(title, departmentId);
            return position == null;
        }

        public async Task<PositionResponseDto?> UpdatePositionAsync(int id, UpdatePositionDto dto)
        {
            var existingPosition = await _positionRepository.GetByIdAsync(id);
            if(existingPosition == null)
            {
                throw new Exception($"There is no position with this Id: '{id}'.");
            }

            existingPosition.Title = dto.Title!;
            existingPosition.DefaultBaseSalary = dto.DefaultBaseSalary ?? 500;
            existingPosition.Description = dto.Description!;
            existingPosition.IsActive = dto.IsActive;
            existingPosition.DepartmentId = dto.DepartmentId;


            var position =  await _positionRepository.UpdatePositionAsync(existingPosition!);

            return new PositionResponseDto()
            {
                Id = position!.Id,
                Title = position.Title,
                DefaultBaseSalary = position.DefaultBaseSalary,
                DepartmentId = position.DepartmentId,
                IsActive = position.IsActive,
                Description = position.Description
            };
        }

    }
}
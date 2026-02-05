using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.Dtos.Shift;
using HRMS.api.entities;
using HRMS.api.Repositories;
using Microsoft.Build.Tasks;

namespace HRMS.api.Services
{
    public interface IShiftService
    {
        Task<ShiftResponseDto> CreateShiftAsync(CreateShiftRequestDto dto);
        Task<bool> DeleteShiftAsync(int id);
        Task<ShiftResponseDto> UpdateShiftAsync(int id, UpdateShiftRequestDto dto);
        Task<ShiftResponseDto> GetShiftByIdAsync(int id);
        Task<List<ShiftResponseDto>> GetAllShiftsAsync();
    }
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;

        public ShiftService(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task<ShiftResponseDto> CreateShiftAsync(CreateShiftRequestDto dto)
        {
            var IsExist = await _shiftRepository.IsShiftExist(null, dto.StartTime, dto.EndTime);
            if(IsExist)
                throw new Exception("Already exists");

            if(dto.RequiredHours < 8) throw new Exception("Shift can't be less than 8 hours");;

            var CreatedShift = new Shift()
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                RequiredHours = dto.RequiredHours
            };
            
            await _shiftRepository.CreateShiftAsync(CreatedShift);

            var shift = new ShiftResponseDto()
            {
                Id = CreatedShift.Id,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                RequiredHours = dto.RequiredHours
            };

            return shift;
        }

        public async Task<bool> DeleteShiftAsync(int id)
        {
            return await _shiftRepository.DeleteShiftAsync(id);
            }

        public async Task<List<ShiftResponseDto>> GetAllShiftsAsync()
        {
            var shifts =  await _shiftRepository.GetAllShiftsAsync();

            return shifts.Select(s=>new ShiftResponseDto()
            {
                Id = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                RequiredHours = s.RequiredHours            
            }).ToList();
        }

        public async Task<ShiftResponseDto> GetShiftByIdAsync(int id)
        {
            var shift =  await _shiftRepository.GetShiftByIdAsync(id);
            if(shift == null)
            {
                throw new Exception($"there is no shift with Id: {id}");
            }
            return new ShiftResponseDto()
            {
                Id = shift.Id,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                RequiredHours = shift.RequiredHours            
            };
        }

        public async Task<ShiftResponseDto> UpdateShiftAsync(int id, UpdateShiftRequestDto dto)
        {
            var IsExist = await _shiftRepository.IsShiftExist(id, dto.StartTime, dto.EndTime);
            if(IsExist)
                throw new Exception("Already exists");

            var shift =  await _shiftRepository.GetShiftByIdAsync(id);
            if(shift == null)
            {
                throw new Exception($"there is no shift with Id: {id}");
            }

            if(dto.RequiredHours < 8) throw new Exception("Shift can't be less than 8 hours");

            shift.StartTime = dto.StartTime;
            shift.EndTime = dto.EndTime;
            shift.RequiredHours = dto.RequiredHours;
            
            await _shiftRepository.UpdateShiftAsync(shift);

            return new ShiftResponseDto()
            {
                Id = shift.Id,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                RequiredHours = shift.RequiredHours            
            };
        }
    }
}
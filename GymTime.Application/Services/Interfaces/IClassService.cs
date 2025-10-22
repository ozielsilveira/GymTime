using GymTime.Application.Dtos.Classes;

namespace GymTime.Application.Services.Interfaces;

public interface IClassService
{
    Task<IEnumerable<ClassDto>> GetAllAsync();
    Task<ClassDto?> GetByIdAsync(Guid id);
    Task<ClassDto> CreateAsync(CreateClassRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateClassRequest request);
    Task<bool> DeleteAsync(Guid id);
}
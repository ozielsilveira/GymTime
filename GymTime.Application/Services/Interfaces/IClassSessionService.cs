using GymTime.Application.Dtos.Classes;

namespace GymTime.Application.Services.Interfaces;

public interface IClassSessionService
{
    Task<IEnumerable<ClassSessionDto>> GetAllAsync();
    Task<ClassSessionDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ClassSessionDto>> GetByClassIdAsync(Guid classId);
    Task<ClassSessionDto> CreateAsync(CreateClassSessionRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateClassSessionRequest request);
    Task<bool> DeleteAsync(Guid id);
}

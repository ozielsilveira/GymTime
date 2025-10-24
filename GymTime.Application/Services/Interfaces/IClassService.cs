using GymTime.Application.Dtos.Classes;

namespace GymTime.Application.Services.Interfaces;

public interface IClassService
{
    Task<IEnumerable<ClassDto>> GetAllAsync();
    Task<ClassDto?> GetByIdAsync(Guid id);
    Task<ClassDto> CreateAsync(CreateClassRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateClassRequest request);
    Task<ClassDto> UpdateWithSessionsAsync(Guid id, UpdateClassWithSessionsRequest request);
    Task<bool> DeleteAsync(Guid id);

    // Methods for session management
    Task<IEnumerable<ClassSessionDto>> GetSessionsByClassIdAsync(Guid classId);
    Task<ClassSessionDto?> GetSessionByIdAsync(Guid classId, Guid sessionId);
    Task<IEnumerable<ClassSessionDto>> AddSessionsToClassAsync(Guid classId, AddSessionsToClassRequest request);
    Task<bool> UpdateSessionAsync(Guid classId, Guid sessionId, UpdateClassSessionRequest request);
    Task<bool> DeleteSessionAsync(Guid classId, Guid sessionId);
}
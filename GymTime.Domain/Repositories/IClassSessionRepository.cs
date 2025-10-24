using GymTime.Domain.Entities;

namespace GymTime.Domain.Repositories;

public interface IClassSessionRepository
{
    Task AddAsync(ClassSession classSession);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ClassSession>> GetAllAsync();
    Task<ClassSession?> GetByIdAsync(Guid id);
    Task<IEnumerable<ClassSession>> GetByClassIdAsync(Guid classId);
    Task UpdateAsync(ClassSession classSession);
}

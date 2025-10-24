using GymTime.Domain.Entities;

namespace GymTime.Domain.Repositories;

public interface IClassRepository
{
    Task AddAsync(Class classEntity);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Class>> GetAllAsync();
    Task<Class?> GetByIdAsync(Guid id);
    Task UpdateAsync(Class classEntity);
}

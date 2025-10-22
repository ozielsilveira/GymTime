using GymTime.Domain.Entities;

namespace GymTime.Domain.Repositories;

public interface IGymMemberRepository
{
    Task DeleteAsync(Guid id);
    Task<IEnumerable<GymMember>> GetAllAsync();
    Task<GymMember?> GetByIdAsync(Guid id);
    Task UpdateAsync(GymMember GymMember);
}

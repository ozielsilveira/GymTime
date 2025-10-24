using GymTime.Application.Dtos.GymMembers;

namespace GymTime.Application.Services.Interfaces;

public interface IGymMemberService
{
    Task<IEnumerable<GymMemberDto>> GetAllAsync();
    Task<GymMemberDto?> GetByIdAsync(Guid id);
    Task<GymMemberDto> CreateAsync(CreateGymMemberRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateGymMemberRequest request);
    Task<bool> DeleteAsync(Guid id);
}

using GymTime.Application.Dtos.Report;

namespace GymTime.Application.Services.Interfaces;

public interface IReportService
{
    Task<ReportDto?> GetGymMemberReportAsync(Guid gymMemberId);
}

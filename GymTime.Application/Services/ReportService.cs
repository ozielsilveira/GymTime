using GymTime.Application.Dtos.Report;
using GymTime.Application.Services.Interfaces;
using GymTime.Domain.Repositories;

namespace GymTime.Application.Services;

public class ReportService(
    IBookingRepository bookingRepository,
    IGymMemberRepository gymMemberRepository) : IReportService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IGymMemberRepository _gymMemberRepository = gymMemberRepository;

    public async Task<ReportDto?> GetGymMemberReportAsync(Guid gymMemberId)
    {
        var gymMember = await _gymMemberRepository.GetByIdAsync(gymMemberId);
        if (gymMember == null)
            return null;

        var bookings = await _bookingRepository.GetBookingsForGymMemberAsync(gymMemberId);
        var now = DateTime.UtcNow;
        var monthBookings = bookings.Where(b => 
        b.ClassSession != null && 
    b.ClassSession.Schedule.Month == now.Month && 
            b.ClassSession.Schedule.Year == now.Year).ToList();

        // Calculate most frequent class types (guard against null Class and ClassSession references)
  var favoriteTypes = monthBookings
     .Where(b => b.Class != null)
     .GroupBy(b => b.Class!.ClassType)
            .Select(g => new { ClassType = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
    .Take(3)
.Select(x => x.ClassType)
            .ToList();

        return new ReportDto
        {
  GymMemberId = gymMember.Id,
   GymMemberName = gymMember.Name,
      PlanType = gymMember.PlanType.ToString(),
    TotalBookingsThisMonth = monthBookings.Count,
            FavoriteClassTypes = favoriteTypes
        };
    }
}
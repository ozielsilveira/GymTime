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
        var monthBookings = bookings.Where(b => b.CreatedAt.Month == now.Month && b.CreatedAt.Year == now.Year).ToList();

        // Calculate most frequent class types (proteção contra Class nulo)
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
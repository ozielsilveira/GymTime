namespace GymTime.Application.Dtos.Report;

/// <summary>
/// Report containing gym member statistics and booking information.
/// </summary>
public class ReportDto
{
    /// <summary>
    /// Identifier of the GymMember this report refers to.
    /// </summary>
    public Guid GymMemberId { get; set; }

    /// <summary>
    /// Gym member name.
    /// </summary>
    public string GymMemberName { get; set; } = string.Empty;

    /// <summary>
    /// Member plan type (e.g., Monthly, Quarterly, Annual).
    /// </summary>
    /// <remarks>
    /// The plan type influences the monthly booking limit shown in the system and the scheduling business rules.
    /// </remarks>
    public string PlanType { get; set; } = string.Empty;

    /// <summary>
    /// Total bookings of the member in the current month (server UTC month/year).
    /// </summary>
    /// <remarks>
    /// This value only considers bookings created in the same month and year in UTC.
    /// </remarks>
    public int TotalBookingsThisMonth { get; set; }

    /// <summary>
    /// List of the member's most frequent class types (ordered by frequency).
    /// </summary>
    /// <remarks>
    /// Used for preference analysis and report generation. May be empty if there are no bookings.
    /// </remarks>
    public List<string> FavoriteClassTypes { get; set; } = [];
}

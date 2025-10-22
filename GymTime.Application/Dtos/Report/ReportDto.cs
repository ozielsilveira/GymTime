namespace GymTime.Application.Dtos.Report;

public class ReportDto
{
    public Guid GymMemberId { get; set; }
    public string GymMemberName { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public int TotalBookingsThisMonth { get; set; }
    public List<string> FavoriteClassTypes { get; set; } = [];
}
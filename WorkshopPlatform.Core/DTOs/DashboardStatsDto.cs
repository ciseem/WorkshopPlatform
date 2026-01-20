namespace WorkshopPlatform.Core.DTOs;

public class DashboardStatsDto
{
    public int TotalWorkshops { get; set; }
    public int ActiveWorkshops { get; set; }
    public int TotalParticipants { get; set; }
    public int TotalInstructors { get; set; }
    public decimal TotalIncome { get; set; }
}

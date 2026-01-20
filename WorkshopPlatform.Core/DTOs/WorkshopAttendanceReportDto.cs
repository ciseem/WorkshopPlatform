namespace WorkshopPlatform.Core.DTOs;

public class WorkshopAttendanceReportDto
{
    public string KatilimciAd { get; set; } = string.Empty;
    public string WorkshopBaslik { get; set; } = string.Empty;
    public DateTime SeansTarih { get; set; }
    public string Durum { get; set; } = string.Empty;
}

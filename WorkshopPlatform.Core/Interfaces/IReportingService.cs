using WorkshopPlatform.Core.DTOs;

namespace WorkshopPlatform.Core.Interfaces;

public interface IReportingService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<IEnumerable<WorkshopAttendanceReportDto>> GetWorkshopAttendanceReportAsync();
    Task<decimal> GetTotalIncomeAsync();
    Task<byte[]> GenerateAttendancePdfReportAsync();
}

using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Core.Interfaces;

public interface IWorkshopService
{
    Task<IEnumerable<Workshop>> GetActiveWorkshopsAsync();
    Task<Workshop?> GetByIdAsync(int id);
    Task<int> CreateWorkshopAsync(Workshop workshop, IEnumerable<WorkshopSeans> seanslar);
    Task UpdateWorkshopAsync(Workshop workshop);
    Task DeleteWorkshopAsync(int id);
    Task<IEnumerable<Workshop>> GetWorkshopsByInstructorAsync(int userId);
    Task<int> GetWorkshopIdBySeansAsync(int seansId);
}

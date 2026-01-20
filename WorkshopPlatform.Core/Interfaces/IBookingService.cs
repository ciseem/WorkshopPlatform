using WorkshopPlatform.Core.Enums;
using WorkshopPlatform.Core.Entities;

namespace WorkshopPlatform.Core.Interfaces;

public interface IBookingService
{
    Task<bool> JoinWorkshopAsync(int userId, int seansId);
    Task<bool> CancelParticipationAsync(int participatonId);
    Task UpdatePaymentStatusAsync(int paymentId, OdemeDurumu durum);
    Task<IEnumerable<WorkshopKatilimci>> GetUserParticipationsAsync(int userId);
}

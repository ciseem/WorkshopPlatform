using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Core.Entities;

public class WorkshopSeans
{
    public int Id { get; set; }
    public int WorkshopId { get; set; }
    public DateTime Tarih { get; set; } 
    public TimeSpan Saat { get; set; } 
    public int Kontenjan { get; set; }
    public int KalanKontenjan { get; set; }

    // Navigation
    public Workshop Workshop { get; set; }
    public ICollection<WorkshopKatilimci> Katilimcilar { get; set; } = new List<WorkshopKatilimci>();
}

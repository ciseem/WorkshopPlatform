using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Core.Entities;

public class WorkshopKatilimci
{
    public int Id { get; set; }
    public int KullaniciId { get; set; }
    public int WorkshopSeansId { get; set; }
    public DateTime KatilimTarihi { get; set; } = DateTime.Now;
    public KatilimDurumu KatilimDurumu { get; set; }

    // Navigation
    public User Kullanici { get; set; }
    public WorkshopSeans Seans { get; set; }
}

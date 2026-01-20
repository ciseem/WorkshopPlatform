using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Core.Entities;

public class Odeme
{
    public int Id { get; set; }
    public int KullaniciId { get; set; }
    public int WorkshopId { get; set; }
    public decimal Tutar { get; set; }
    public OdemeDurumu OdemeDurumu { get; set; }
    public DateTime OdemeTarihi { get; set; } = DateTime.Now;

    // Navigation
    public User Kullanici { get; set; }
    public Workshop Workshop { get; set; }
}

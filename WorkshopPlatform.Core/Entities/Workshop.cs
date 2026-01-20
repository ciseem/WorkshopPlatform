using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Core.Entities;

public class Workshop
{
    public int Id { get; set; }
    public string Baslik { get; set; }
    public string Aciklama { get; set; }
    public string Kategori { get; set; }
    public bool UcretliMi { get; set; }
    public decimal? Fiyat { get; set; }
    public int OlusturanKullaniciId { get; set; }
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    public WorkshopDurum Durum { get; set; }

    // Location Info ⚓🗺️
    public string? Sehir { get; set; }
    public string? Ilce { get; set; }
    public string? MekanAd { get; set; }
    public decimal? Enlem { get; set; }
    public decimal? Boylam { get; set; }

    // Navigation
    public User OlusturanKullanici { get; set; }
    public ICollection<WorkshopSeans> Seanslar { get; set; } = new List<WorkshopSeans>();
}

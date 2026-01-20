using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Core.Entities;

public class User
{
    public int Id { get; set; }

    public string AdSoyad { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? Telefon { get; set; }
    public string? ProfileImage { get; set; } // New Profile Image Path
    public UserRole Role { get; set; }

    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

    // Navigation Properties
    public ICollection<Workshop> OlusturulanWorkshoplar { get; set; } = new List<Workshop>();
    public ICollection<WorkshopKatilimci> Katilimlar { get; set; } = new List<WorkshopKatilimci>();
    public ICollection<Odeme> Odemeler { get; set; } = new List<Odeme>();
}

using Microsoft.EntityFrameworkCore;
using WorkshopPlatform.Core.Entities;

namespace WorkshopPlatform.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<WorkshopSeans> WorkshopSeanslari { get; set; }
    public DbSet<WorkshopKatilimci> WorkshopKatilimcilari { get; set; }
    public DbSet<Odeme> Odemeler { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.AdSoyad).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefon).HasMaxLength(20);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).HasConversion<int>().IsRequired();
        });

        // Workshops
        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.ToTable("Workshops");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Baslik).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Fiyat).HasColumnType("decimal(18,2)");
            
            // FK -> User
            entity.HasOne(d => d.OlusturanKullanici)
                .WithMany(p => p.OlusturulanWorkshoplar)
                .HasForeignKey(d => d.OlusturanKullaniciId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // WorkshopSeanslari
        modelBuilder.Entity<WorkshopSeans>(entity =>
        {
            entity.ToTable("WorkshopSeanslari");
            entity.HasKey(e => e.Id);
            
            // FK -> Workshop
            entity.HasOne(d => d.Workshop)
                .WithMany(p => p.Seanslar)
                .HasForeignKey(d => d.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkshopKatilimcilari
        modelBuilder.Entity<WorkshopKatilimci>(entity =>
        {
            entity.ToTable("WorkshopKatilimcilari");
            entity.HasKey(e => e.Id);

            // FK -> User
            entity.HasOne(d => d.Kullanici)
                .WithMany(p => p.Katilimlar)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK -> Seans
            entity.HasOne(d => d.Seans)
                .WithMany(p => p.Katilimcilar)
                .HasForeignKey(d => d.WorkshopSeansId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Odemeler
        modelBuilder.Entity<Odeme>(entity =>
        {
            entity.ToTable("Odemeler");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tutar).HasColumnType("decimal(18,2)");

            // FK -> User
            entity.HasOne(d => d.Kullanici)
                .WithMany(p => p.Odemeler)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK -> Workshop
            entity.HasOne(d => d.Workshop)
                .WithMany() // No collection on Workshop for Odemeler requested but good to have constraint
                .HasForeignKey(d => d.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

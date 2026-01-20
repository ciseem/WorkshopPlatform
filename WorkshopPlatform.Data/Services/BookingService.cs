using Microsoft.EntityFrameworkCore;
using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.Enums;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Data.Context;

namespace WorkshopPlatform.Data.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> JoinWorkshopAsync(int userId, int seansId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var seans = await _context.WorkshopSeanslari
                .Include(s => s.Workshop)
                .FirstOrDefaultAsync(s => s.Id == seansId);

            if (seans == null || seans.KalanKontenjan <= 0) return false;

            // Check if already joined
            var exists = await _context.WorkshopKatilimcilari
                .AnyAsync(k => k.KullaniciId == userId && k.WorkshopSeansId == seansId);
            
            if (exists) return false;

            // Create participation
            var katilim = new WorkshopKatilimci
            {
                KullaniciId = userId,
                WorkshopSeansId = seansId,
                KatilimDurumu = KatilimDurumu.Kayitli
            };

            _context.WorkshopKatilimcilari.Add(katilim);

            // Update quota
            seans.KalanKontenjan--;

            // Handle Payment record if Paid
            if (seans.Workshop.UcretliMi)
            {
                var odeme = new Odeme
                {
                    KullaniciId = userId,
                    WorkshopId = seans.WorkshopId,
                    Tutar = seans.Workshop.Fiyat ?? 0,
                    OdemeDurumu = OdemeDurumu.Beklemede
                };
                _context.Odemeler.Add(odeme);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> CancelParticipationAsync(int participatonId)
    {
        var katilim = await _context.WorkshopKatilimcilari
            .Include(k => k.Seans)
            .FirstOrDefaultAsync(k => k.Id == participatonId);

        if (katilim == null) return false;

        katilim.KatilimDurumu = KatilimDurumu.Iptal;
        katilim.Seans.KalanKontenjan++;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task UpdatePaymentStatusAsync(int paymentId, OdemeDurumu durum)
    {
        var odeme = await _context.Odemeler.FindAsync(paymentId);
        if (odeme != null)
        {
            odeme.OdemeDurumu = durum;
            odeme.OdemeTarihi = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<WorkshopKatilimci>> GetUserParticipationsAsync(int userId)
    {
        return await _context.WorkshopKatilimcilari
            .Include(k => k.Seans)
                .ThenInclude(s => s.Workshop)
                    .ThenInclude(w => w.OlusturanKullanici)
            .Where(k => k.KullaniciId == userId)
            .OrderByDescending(k => k.KatilimTarihi)
            .ToListAsync();
    }
}

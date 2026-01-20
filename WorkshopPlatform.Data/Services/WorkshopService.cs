using Microsoft.EntityFrameworkCore;
using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.Enums;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Data.Context;

namespace WorkshopPlatform.Data.Services;

public class WorkshopService : IWorkshopService
{
    private readonly AppDbContext _context;

    public WorkshopService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Workshop>> GetActiveWorkshopsAsync()
    {
        return await _context.Workshops
            .Include(w => w.OlusturanKullanici)
            .Include(w => w.Seanslar)
            .Where(w => w.Durum == WorkshopDurum.Aktif)
            .OrderByDescending(w => w.OlusturulmaTarihi)
            .ToListAsync();
    }

    public async Task<Workshop?> GetByIdAsync(int id)
    {
        return await _context.Workshops
            .Include(w => w.OlusturanKullanici)
            .Include(w => w.Seanslar)
                .ThenInclude(s => s.Katilimcilar)
                    .ThenInclude(k => k.Kullanici)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<int> CreateWorkshopAsync(Workshop workshop, IEnumerable<WorkshopSeans> seanslar)
    {
        _context.Workshops.Add(workshop);
        await _context.SaveChangesAsync();

        foreach (var seans in seanslar)
        {
            seans.WorkshopId = workshop.Id;
            // KalanKontenjan initial value should be Kontenjan
            seans.KalanKontenjan = seans.Kontenjan;
            _context.WorkshopSeanslari.Add(seans);
        }

        await _context.SaveChangesAsync();
        return workshop.Id;
    }

    public async Task UpdateWorkshopAsync(Workshop workshop)
    {
        _context.Workshops.Update(workshop);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkshopAsync(int id)
    {
        var workshop = await _context.Workshops
            .Include(w => w.Seanslar)
                .ThenInclude(s => s.Katilimcilar)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workshop != null)
        {
            // 1. Delete all participants in all sessions
            foreach (var seans in workshop.Seanslar)
            {
                if (seans.Katilimcilar != null && seans.Katilimcilar.Any())
                {
                    _context.WorkshopKatilimcilari.RemoveRange(seans.Katilimcilar);
                }
            }

            // 2. Delete related payments (if any)
            var odemeler = await _context.Odemeler.Where(o => o.WorkshopId == id).ToListAsync();
            if (odemeler.Any())
            {
                _context.Odemeler.RemoveRange(odemeler);
            }

            // 3. Delete the workshop (Cascades to Sessions)
            _context.Workshops.Remove(workshop);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Workshop>> GetWorkshopsByInstructorAsync(int userId)
    {
        return await _context.Workshops
            .Where(w => w.OlusturanKullaniciId == userId)
            .Include(w => w.Seanslar)
            .ToListAsync();
    }

    public async Task<int> GetWorkshopIdBySeansAsync(int seansId)
    {
        var seans = await _context.WorkshopSeanslari.FindAsync(seansId);
        return seans?.WorkshopId ?? 1;
    }
}

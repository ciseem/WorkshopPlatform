using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Web.Controllers;

[Authorize(Roles = "Instructor")]
public class InstructorController : Controller
{
    private readonly IWorkshopService _workshopService;

    public InstructorController(IWorkshopService workshopService)
    {
        _workshopService = workshopService;
    }

    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return RedirectToAction("Login", "Auth"); 
        }

        var workshops = await _workshopService.GetWorkshopsByInstructorAsync(userId);
        
        var activeWorkshops = workshops.Count(w => w.Durum == WorkshopDurum.Aktif);
        var totalStudents = workshops.SelectMany(w => w.Seanslar).Sum(s => s.Kontenjan - s.KalanKontenjan);
        var potentialIncome = workshops.Where(w => w.UcretliMi).Sum(w => (w.Fiyat ?? 0) * w.Seanslar.Sum(s => s.Kontenjan - s.KalanKontenjan));

        ViewBag.ActiveWorkshops = activeWorkshops;
        ViewBag.TotalStudents = totalStudents;
        ViewBag.TotalIncome = potentialIncome;

        return View(workshops);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Workshop workshop, DateTime tarih, TimeSpan saat, int kontenjan, string sehir, string ilce, string mekanAd, decimal? enlem, decimal? boylam)
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value!);
        workshop.OlusturanKullaniciId = userId;
        workshop.Durum = WorkshopDurum.Aktif;
        workshop.OlusturulmaTarihi = DateTime.Now;
        
        workshop.Sehir = sehir;
        workshop.Ilce = ilce;
        workshop.MekanAd = mekanAd;
        workshop.Enlem = enlem;
        workshop.Boylam = boylam;

        // Create default session
        var defaultSeans = new List<WorkshopSeans> 
        { 
            new WorkshopSeans 
            { 
                Tarih = tarih,
                Saat = saat,
                Kontenjan = kontenjan,
                KalanKontenjan = kontenjan
            }
        };

        await _workshopService.CreateWorkshopAsync(workshop, defaultSeans);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var workshop = await _workshopService.GetByIdAsync(id);
        if (workshop == null) return NotFound();
        return View(workshop);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Workshop model)
    {
        await _workshopService.UpdateWorkshopAsync(model);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _workshopService.DeleteWorkshopAsync(id);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Participants(int id)
    {
        var workshop = await _workshopService.GetByIdAsync(id);
        if (workshop == null) return NotFound();

        // Security: Ensure the instructor owns this workshop
        var userId = int.Parse(User.FindFirst("UserId")?.Value!);
        if (workshop.OlusturanKullaniciId != userId) return Forbid();

        return View(workshop);
    }
}

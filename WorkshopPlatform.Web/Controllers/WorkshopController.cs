using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Web.Controllers;

[Authorize]
public class WorkshopController : Controller
{
    private readonly IWorkshopService _workshopService;
    private readonly IBookingService _bookingService;

    public WorkshopController(IWorkshopService workshopService, IBookingService bookingService)
    {
        _workshopService = workshopService;
        _bookingService = bookingService;
    }

    public async Task<IActionResult> Index()
    {
        var workshops = await _workshopService.GetActiveWorkshopsAsync();
        return View(workshops);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var workshop = await _workshopService.GetByIdAsync(id);
        if (workshop == null) return NotFound();
        return View(workshop);
    }

    [HttpPost]
    public async Task<IActionResult> Join(int seansId)
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return RedirectToAction("Login", "Auth"); 
        }

        var success = await _bookingService.JoinWorkshopAsync(userId, seansId);
        
        if (success)
            TempData["Message"] = "Katılım kaydınız başarıyla oluşturuldu.";
        else
            TempData["Error"] = "Kapasite dolu veya zaten kayıtlısınız.";

        var workshopId = await _workshopService.GetWorkshopIdBySeansAsync(seansId);
        return RedirectToAction("Detail", new { id = workshopId });
    }
}

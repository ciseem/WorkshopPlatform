using Microsoft.AspNetCore.Mvc;
using WorkshopPlatform.Core.Interfaces;

namespace WorkshopPlatform.Web.Controllers;

public class UserController : Controller
{
    private readonly IWorkshopService _workshopService;
    private readonly IBookingService _bookingService;

    public UserController(IWorkshopService workshopService, IBookingService bookingService)
    {
        _workshopService = workshopService;
        _bookingService = bookingService;
    }

    public async Task<IActionResult> Workshops()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        var createdWorkshops = await _workshopService.GetWorkshopsByInstructorAsync(userId);
        var joinedParticipations = await _bookingService.GetUserParticipationsAsync(userId);

        ViewBag.CreatedWorkshops = createdWorkshops;
        ViewBag.JoinedParticipations = joinedParticipations;

        return View();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting; // For IWebHostEnvironment
using Microsoft.AspNetCore.Http; // For IFormFile
using System;
using System.IO;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Core.Entities;

namespace WorkshopPlatform.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _environment;

    public ProfileController(IUserService userService, IWebHostEnvironment environment)
    {
        _userService = userService;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (email == null) return RedirectToAction("Login", "Auth");

        var user = await _userService.GetByEmailAsync(email);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> UploadPhoto(IFormFile photo)
    {
        if (photo == null || photo.Length == 0)
        {
            TempData["Error"] = "Lütfen geçerli bir fotoğraf seçin.";
            return RedirectToAction("Index");
        }

        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _userService.GetByEmailAsync(email!);

        if (user == null) return NotFound();

        // Unique filename
        var fileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "profiles");

        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        // Update User Entity (Ideally via Service)
        user.ProfileImage = $"/uploads/profiles/{fileName}";
        
        // Quick update logic via UserService (requires Update method) 
        // For now, we assume direct context access is not available here, so we might need a method in UserService.
        await _userService.UpdateProfileImageAsync(user.Id, user.ProfileImage);

        TempData["Success"] = "Profil fotoğrafınız güncellendi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount()
    {
        var userIdStr = User.FindFirst("UserId")?.Value;
        if (int.TryParse(userIdStr, out var userId))
        {
            await _userService.DeleteAccountAsync(userId);
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Auth");
        }
        return RedirectToAction("Index");
    }
}

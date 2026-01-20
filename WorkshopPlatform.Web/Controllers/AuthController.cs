using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.Enums;

namespace WorkshopPlatform.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string role)
    {
        var user = await _userService.LoginAsync(email, password);

        if (user != null)
        {
            if (user.Role.ToString() != role)
            {
                ViewBag.Error = $"Bu portal sadece { (role == "Instructor" ? "Eğitmenler" : "Katılımcılar") } içindir.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.AdSoyad),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("ProfileImage", user.ProfileImage ?? "")
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            if (user.Role == UserRole.Instructor)
            {
                return RedirectToAction("Index", "Instructor");
            }

            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Error = "Geçersiz email veya şifre.";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string adSoyad, string email, string password, string telefon, UserRole role)
    {
        var existingUser = await _userService.GetByEmailAsync(email);
        if (existingUser != null)
        {
            ViewBag.Error = "Bu e-posta adresi zaten kullanımda.";
            return View();
        }

        var user = new User
        {
            AdSoyad = adSoyad,
            Email = email,
            Telefon = telefon,
            Role = role
        };

        await _userService.RegisterAsync(user, password);

        TempData["Success"] = "Hesabınız başarıyla oluşturulmuştur. Lütfen giriş yapınız.";
        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var token = await _userService.GeneratePasswordResetTokenAsync(email);
        if (token != null)
        {
            TempData["Success"] = $"Şifre sıfırlama kodunuz: {token}";
            return RedirectToAction("ResetPassword", new { email });
        }

        ViewBag.Error = "Bu e-posta adresine kayıtlı kullanıcı bulunamadı.";
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        ViewBag.Email = email;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
    {
        var success = await _userService.ResetPasswordAsync(email, token, newPassword);
        if (success)
        {
            TempData["Success"] = "Şifre başarıyla güncellendi. Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        ViewBag.Error = "Geçersiz veya süresi dolmuş kod.";
        ViewBag.Email = email;
        return View();
    }
}

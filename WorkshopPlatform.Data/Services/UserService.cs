using Microsoft.EntityFrameworkCore;
using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Data.Context;

namespace WorkshopPlatform.Data.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterAsync(User user, string password)
    {
        // In a real app, hash the password! For this demo, we'll store it plain or simple hash
        user.Password = password; 
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await GetByEmailAsync(email);
        if (user == null) return null;

        var token = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        user.ResetToken = token;
        user.ResetTokenExpiry = DateTime.Now.AddHours(1);

        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || user.ResetToken != token || user.ResetTokenExpiry < DateTime.Now)
            return false;

        user.Password = newPassword; 
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task UpdateProfileImageAsync(int userId, string imagePath)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.ProfileImage = imagePath;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAccountAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.OlusturulanWorkshoplar)
                .ThenInclude(w => w.Seanslar)
                    .ThenInclude(s => s.Katilimcilar)
            .Include(u => u.Katilimlar)
            .Include(u => u.Odemeler)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            // 1. If Instructor, delete their workshops (and related data)
            foreach (var workshop in user.OlusturulanWorkshoplar.ToList())
            {
                // We use the same cleanup logic as WorkshopService.DeleteWorkshopAsync
                foreach (var seans in workshop.Seanslar)
                {
                    if (seans.Katilimcilar.Any())
                        _context.WorkshopKatilimcilari.RemoveRange(seans.Katilimcilar);
                }
                
                var workshopOdemeler = await _context.Odemeler.Where(o => o.WorkshopId == workshop.Id).ToListAsync();
                _context.Odemeler.RemoveRange(workshopOdemeler);
                _context.Workshops.Remove(workshop);
            }

            // 2. If Student, clean their participations and payments
            if (user.Katilimlar.Any())
                _context.WorkshopKatilimcilari.RemoveRange(user.Katilimlar);
            
            if (user.Odemeler.Any())
                _context.Odemeler.RemoveRange(user.Odemeler);

            // 3. Delete the user
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

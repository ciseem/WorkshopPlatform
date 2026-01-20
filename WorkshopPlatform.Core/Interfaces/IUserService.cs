using WorkshopPlatform.Core.Entities;

namespace WorkshopPlatform.Core.Interfaces;

public interface IUserService
{
    Task<User> RegisterAsync(User user, string password);
    Task<User?> LoginAsync(string email, string password);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<string?> GeneratePasswordResetTokenAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task UpdateProfileImageAsync(int userId, string imagePath);
    Task DeleteAccountAsync(int userId);
}

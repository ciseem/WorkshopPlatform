using Microsoft.EntityFrameworkCore;
using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.Enums;
using WorkshopPlatform.Data.Context;

namespace WorkshopPlatform.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Otomatik demo veri ekleme devre dışı bırakıldı.
        // Artık veritabanı tamamen kullanıcı kontrolünde.
        await Task.CompletedTask;
    }
}

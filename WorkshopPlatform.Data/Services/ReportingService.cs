using Microsoft.EntityFrameworkCore;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Data.Context;
using WorkshopPlatform.Core.Enums;
using WorkshopPlatform.Core.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace WorkshopPlatform.Data.Services;

public class ReportingService : IReportingService
{
    private readonly AppDbContext _context;

    public ReportingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalWorkshops = await _context.Workshops.CountAsync(),
            ActiveWorkshops = await _context.Workshops.CountAsync(w => w.Durum == WorkshopDurum.Aktif),
            TotalParticipants = await _context.WorkshopKatilimcilari.CountAsync(),
            TotalInstructors = await _context.Users.CountAsync(u => u.Role == Core.Enums.UserRole.Instructor),
            TotalIncome = await GetTotalIncomeAsync()
        };
    }

    public async Task<IEnumerable<WorkshopAttendanceReportDto>> GetWorkshopAttendanceReportAsync()
    {
        return await _context.WorkshopKatilimcilari
            .Include(k => k.Kullanici)
            .Include(k => k.Seans)
                .ThenInclude(s => s.Workshop)
            .Select(k => new WorkshopAttendanceReportDto
            {
                KatilimciAd = k.Kullanici.AdSoyad,
                WorkshopBaslik = k.Seans.Workshop.Baslik,
                SeansTarih = k.Seans.Tarih,
                Durum = k.KatilimDurumu.ToString()
            })
            .ToListAsync();
    }

    public async Task<decimal> GetTotalIncomeAsync()
    {
        return await _context.Odemeler
            .Where(o => o.OdemeDurumu == Core.Enums.OdemeDurumu.Odendi)
            .SumAsync(o => o.Tutar);
    }

    public async Task<byte[]> GenerateAttendancePdfReportAsync()
    {
        var data = await GetWorkshopAttendanceReportAsync();
        
        using var stream = new MemoryStream();
        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(QuestPDF.Helpers.PageSizes.A4);
                page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                page.PageColor(QuestPDF.Helpers.Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("WORKSHOP PLATFORM").FontSize(20).SemiBold().FontColor(QuestPDF.Helpers.Colors.Blue.Medium);
                        col.Item().Text("Katılım ve Performans Raporu").FontSize(12).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Tarih: {DateTime.Now:dd.MM.yyyy}");
                        col.Item().Text($"Saat: {DateTime.Now:HH:mm}");
                    });
                });

                page.Content().PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(x =>
                {
                    x.Spacing(10);
                    x.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Katılımcı");
                            header.Cell().Element(HeaderStyle).Text("Workshop");
                            header.Cell().Element(HeaderStyle).Text("Tarih");
                            header.Cell().Element(HeaderStyle).Text("Durum");

                            static QuestPDF.Infrastructure.IContainer HeaderStyle(QuestPDF.Infrastructure.IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Black);
                            }
                        });

                        foreach (var item in data)
                        {
                            table.Cell().Element(CellStyle).Text(item.KatilimciAd);
                            table.Cell().Element(CellStyle).Text(item.WorkshopBaslik);
                            table.Cell().Element(CellStyle).Text(item.SeansTarih.ToString("dd.MM.yyyy"));
                            table.Cell().Element(CellStyle).Text(item.Durum);

                            static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5);
                            }
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Sayfa ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf(stream);

        return stream.ToArray();
    }
}

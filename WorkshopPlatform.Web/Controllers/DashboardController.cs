using Microsoft.AspNetCore.Mvc;
using WorkshopPlatform.Core.Interfaces;
using WorkshopPlatform.Data.Services;
using WorkshopPlatform.Core.Entities;
using WorkshopPlatform.Core.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace WorkshopPlatform.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IReportingService _reportingService;
    private readonly IWorkshopService _workshopService;

    public DashboardController(IReportingService reportingService, IWorkshopService workshopService)
    {
        _reportingService = reportingService;
        _workshopService = workshopService;
    }

    public async Task<IActionResult> Index()
    {
        // If Instructor tries to access student dashboard, redirect them to their own panel
        if (User.IsInRole("Instructor"))
        {
            return RedirectToAction("Index", "Instructor");
        }

        var stats = await _reportingService.GetDashboardStatsAsync();
        var activeWorkshops = await _workshopService.GetActiveWorkshopsAsync();

        // For students, we might want to hide sensitive global stats or replace them
        // For now, we keep generic stats but we will hide Income in the View
        
        ViewBag.Stats = stats;
        return View(activeWorkshops);
    }

    public async Task<IActionResult> Reports()
    {
        var report = await _reportingService.GetWorkshopAttendanceReportAsync();
        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadCsv()
    {
        var reportData = await _reportingService.GetWorkshopAttendanceReportAsync();
        
        var csvBuilder = new System.Text.StringBuilder();
        // UTF-8 with BOM for Excel compatibility
        csvBuilder.AppendLine("Katilimci;Workshop;Tarih;Durum");

        foreach (var item in reportData)
        {
            csvBuilder.AppendLine($"{item.KatilimciAd};{item.WorkshopBaslik};{item.SeansTarih:dd.MM.yyyy};{item.Durum}");
        }

        var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(csvBuilder.ToString())).ToArray();
        return File(bytes, "text/csv", $"Workshop_Raporu_{DateTime.Now:yyyyMMdd}.csv");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadPdf()
    {
        var pdfBytes = await _reportingService.GenerateAttendancePdfReportAsync();
        return File(pdfBytes, "application/pdf", $"Workshop_Raporu_{DateTime.Now:yyyyMMdd}.pdf");
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/archive")]
public class ArchiveController : ControllerBase
{
    private readonly TmsDbContext _context;

    public ArchiveController(TmsDbContext context)
    {
        _context = context;
    }

    // ========== SESSION 3: Bulk Archive with ExecuteUpdateAsync ==========
    [HttpPost("old-enrollments")]
    public async Task<IActionResult> ArchiveOldEnrollments(
        [FromQuery] int daysOld = 365)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);

        Console.WriteLine($"\n=== BULK ARCHIVE: Archiving enrollments older than {cutoffDate} ===");

        // ExecuteUpdateAsync generates a single UPDATE statement
        var updatedCount = await _context.Enrollments
            .Where(e => e.EnrolledAt < cutoffDate)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.IsArchived, true)
            );

        Console.WriteLine($"Archived {updatedCount} enrollments with a single UPDATE statement\n");

        return Ok(new
        {
            ArchivedCount = updatedCount,
            CutoffDate = cutoffDate,
            Message = "Bulk archive completed"
        });
    }

    // ========== SESSION 3: View Archived Enrollments ==========
    [HttpGet("archived")]
    public async Task<IActionResult> GetArchivedEnrollments()
    {
        var archived = await _context.Enrollments
            .Where(e => e.IsArchived)
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Select(e => new
            {
                e.Id,
                StudentName = e.Student.Name,
                CourseTitle = e.Course.Title,
                e.EnrolledAt,
                e.Grade
            })
            .ToListAsync();

        return Ok(archived);
    }
}
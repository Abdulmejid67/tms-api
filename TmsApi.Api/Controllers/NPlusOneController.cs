using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/nplusone")]
public class NPlusOneController : ControllerBase
{
    private readonly TmsDbContext _context;

    public NPlusOneController(TmsDbContext context)
    {
        _context = context;
    }

    // ========== SESSION 3: N+1 Anti-Pattern ==========
    [HttpGet("bad")]
    public async Task<IActionResult> GetNPlusOneBad()
    {
        Console.WriteLine("\n=== N+1 ANTI-PATTERN ===");
        var results = new List<object>();

        // Step 1: Load all students (1 query)
        var students = await _context.Students.AsNoTracking().ToListAsync();
        Console.WriteLine($"Query 1: Loaded {students.Count} students");

        var queryCount = 1;

        // Step 2: For each student, load their enrollments (N queries)
        foreach (var s in students)
        {
            var count = await _context.Enrollments
                .AsNoTracking()
                .CountAsync(e => e.StudentId == s.Id);

            Console.WriteLine($"Query {++queryCount}: Counted enrollments for {s.Name} ({count})");
            results.Add(new { s.Name, EnrollmentCount = count });
        }

        Console.WriteLine($"Total queries: {queryCount} (1 + N)\n");
        return Ok(results);
    }

    // ========== SESSION 3: N+1 Fix with Projection ==========
    [HttpGet("good")]
    public async Task<IActionResult> GetNPlusOneGood()
    {
        Console.WriteLine("\n=== FIXED WITH PROJECTION ===");

        // Single query with subquery
        var results = await _context.Students
            .AsNoTracking()
            .Select(s => new
            {
                s.Name,
                EnrollmentCount = s.Enrollments.Count  // Translates to SQL subquery
            })
            .ToListAsync();

        Console.WriteLine($"Total queries: 1 (with subquery)\n");
        return Ok(results);
    }

    // ========== SESSION 3: N+1 Fix with Include ==========
    [HttpGet("include")]
    public async Task<IActionResult> GetNPlusOneInclude()
    {
        Console.WriteLine("\n=== FIXED WITH INCLUDE ===");

        // Single query with JOIN
        var students = await _context.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
            .ToListAsync();

        var results = students.Select(s => new
        {
            s.Name,
            EnrollmentCount = s.Enrollments.Count
        });

        Console.WriteLine($"Total queries: 1 (with JOIN)\n");
        return Ok(results);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly TmsDbContext _context;

    public TestController(TmsDbContext context)
    {
        _context = context;
    }

    // ========== SESSION 1: Deferred Execution Demo ==========
    [HttpGet("deferred")]
    public async Task<IActionResult> TestDeferred()
    {
        Console.WriteLine("\n>> STEP 1: Building the query object (no database contact)...");
        var query = _context.Students.Where(s => s.GPA >= 3.0m);

        Console.WriteLine("\n>> STEP 2: Appending a sorting clause...");
        var orderedQuery = query.OrderBy(s => s.Name);

        Console.WriteLine(">> STEP 3: Materializing query into a C# List...");
        var results = await orderedQuery.ToListAsync();  // Execution triggered here

        Console.WriteLine(">> STEP 4: Materialization finished. List populated.\n");
        return Ok(results);
    }

    // ========== SESSION 1: Translation Failure Demo ==========
    private static bool IsHonorRoll(decimal gpa) => gpa >= 3.5m;

    [HttpGet("translation-fail")]
    public async Task<IActionResult> TestTranslationFail()
    {
        Console.WriteLine("\n>> STEP 1: Running non-translatable query...");
        try
        {
            var students = await _context.Students
                .Where(s => IsHonorRoll(s.GPA))  // This will fail
                .ToListAsync();
            return Ok(students);
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");
            return BadRequest(new { Message = ex.Message });
        }
    }

    // ========== SESSION 1: Fixed Translation ==========
    [HttpGet("translation-fixed")]
    public async Task<IActionResult> TestTranslationFixed()
    {
        Console.WriteLine("\n>> STEP 1: Running translatable query...");
        var students = await _context.Students
            .Where(s => s.GPA >= 3.5m)  // Inline expression - works!
            .ToListAsync();

        Console.WriteLine($">>> Found {students.Count} honor roll students\n");
        return Ok(students);
    }

    // ========== SESSION 1: Client Evaluation (Performance Trap) ==========
    [HttpGet("client-eval")]
    public IActionResult TestClientEvaluation()
    {
        Console.WriteLine("\n>> STEP 1: Running query with client evaluation...");

        // This pulls ALL students into memory first!
        // Use .ToList() instead of .ToListAsync() when using AsEnumerable()
        var students = _context.Students
            .AsEnumerable()  // Forces client evaluation
            .Where(s => IsHonorRoll(s.GPA))
            .ToList();  // Use ToList() not ToListAsync()

        Console.WriteLine($">>> Found {students.Count} honor roll students\n");
        return Ok(students);
    }

    // ========== SESSION 1: Alternative - Convert to List First ==========
    [HttpGet("client-eval-v2")]
    public IActionResult TestClientEvaluationV2()
    {
        Console.WriteLine("\n>> STEP 1: Running query with client evaluation (v2)...");

        // Alternative: Convert to List first, then filter in memory
        var allStudents = _context.Students.ToList();  // Pulls all students
        var honorStudents = allStudents.Where(s => IsHonorRoll(s.GPA)).ToList();

        Console.WriteLine($">>> Found {honorStudents.Count} honor roll students\n");
        return Ok(honorStudents);
    }
}
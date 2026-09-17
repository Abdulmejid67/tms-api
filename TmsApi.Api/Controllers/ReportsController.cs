using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly TmsDbContext _context;

    public ReportsController(TmsDbContext context)
    {
        _context = context;
    }

    // ========== SESSION 1: Query 1 - Active Students with GPA >= 3.0 ==========
    [HttpGet("active-high-gpa")]
    public async Task<IActionResult> GetActiveHighGpaStudents()
    {
        var count = await _context.Students
            .Where(s => s.IsActive && s.GPA >= 3.0m)
            .CountAsync();

        return Ok(new { ActiveHighGpaCount = count });
    }

    // ========== SESSION 1: Query 2 - Courses by Enrollment Count ==========
    [HttpGet("courses-by-enrollment")]
    public async Task<IActionResult> GetCoursesByEnrollment()
    {
        var list = await _context.Courses
            .Select(c => new
            {
                c.Title,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .ToListAsync();

        return Ok(list);
    }

    // ========== SESSION 1: Query 3 - Average GPA per Course ==========
    [HttpGet("average-gpa-by-course")]
    public async Task<IActionResult> GetAverageGpaByCourse()
    {
        var list = await _context.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                Course = g.Key,
                AverageGPA = g.Average(e => e.Student.GPA)
            })
            .ToListAsync();

        return Ok(list);
    }

    // ========== SESSION 1: Query 4 - Students with Zero Enrollments ==========
    [HttpGet("zero-enrollments")]
    public async Task<IActionResult> GetStudentsWithZeroEnrollments()
    {
        var studentsWithNoEnrollments = await _context.Students
            .Where(s => !s.Enrollments.Any())
            .Select(s => s.Name)
            .ToListAsync();

        return Ok(studentsWithNoEnrollments);
    }

    // ========== SESSION 2: Pagination ==========
    [HttpGet("paged-students")]
    public async Task<IActionResult> GetPagedStudents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var students = await _context.Students
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            Students = students
        });
    }

    // ========== SESSION 2: Top 5 Courses by Enrollment ==========
    [HttpGet("top-courses")]
    public async Task<IActionResult> GetTopCourses()
    {
        var topCourses = await _context.Courses
            .Select(c => new
            {
                c.Title,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToListAsync();

        return Ok(topCourses);
    }
}
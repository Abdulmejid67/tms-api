using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Dtos;
using TmsApi.Application.Interfaces;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/enrollments")]
[Tags("Enrollments")]
[Produces("application/json")]
public class EnrollmentsController(
    ICourseService courseService,
    IEnrollmentService enrollmentService) : ControllerBase
{
    // GET /api/courses/{courseId}/enrollments
    [HttpGet(Name = "ListCourseEnrollments")]
    public async Task<IActionResult> GetEnrollments(int courseId, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(courseId, ct);
        if (course is null) return NotFound();

        var enrollments = await enrollmentService.GetByCourseAsync(courseId, ct);

        var result = enrollments.Select(e => new EnrollmentResponseDto(
            e.Id, e.CourseId, e.StudentId, e.EnrolledAt));

        return Ok(result);
    }

    // GET /api/courses/{courseId}/enrollments/{id}
    [HttpGet("{id:int}", Name = nameof(GetEnrollment))]
    public async Task<IActionResult> GetEnrollment(int courseId, int id, CancellationToken ct)
    {
        // Uses the 3-arg overload
        var enrollment = await enrollmentService.GetByIdAsync(courseId, id, ct);
        if (enrollment is null) return NotFound();

        var dto = new EnrollmentResponseDto(
            enrollment.Id, enrollment.CourseId, enrollment.StudentId, enrollment.EnrolledAt);

        return Ok(dto);
    }

    // POST /api/courses/{courseId}/enrollments
    [HttpPost]
    public async Task<IActionResult> EnrollStudent(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct)
    {
        // 1. Parent course exists?
        var course = await courseService.GetByIdAsync(courseId, ct);
        if (course is null) return NotFound();

        // 2. Capacity check — use Enrollments.Count, not a nonexistent property
        var currentCount = course.Enrollments?.Count ?? 0;
        if (currentCount >= course.MaxCapacity)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course is full",
                Detail = $"Course '{course.Title}' has reached its maximum capacity of {course.MaxCapacity}.",
                Status = StatusCodes.Status409Conflict
            });
        }

        // 3. Create the enrollment
        var created = await enrollmentService.CreateAsync(courseId, request.StudentId, ct);

        var dto = new EnrollmentResponseDto(
            created.Id, created.CourseId, created.StudentId, created.EnrolledAt);

        return CreatedAtAction(nameof(GetEnrollment), new { courseId, id = dto.Id }, dto);
    }
}
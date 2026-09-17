using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Dtos;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/courses")]
[Tags("Courses")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class CoursesController(
    ICourseService courseService,
    LinkGenerator linkGenerator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(
        [FromQuery] PagedRequest request,
        CancellationToken ct)
    {
        var result = await courseService.GetCoursesAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = nameof(GetCourseById))]
    public async Task<IActionResult> GetCourseById(int id, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(id, ct);
        if (course is null) return NotFound();

        // Course entity does NOT have EnrollmentCount — use Enrollments.Count
        var enrollmentCount = course.Enrollments?.Count ?? 0;

        var dto = new CourseResponseDto(
            course.Id, course.Code, course.Title,
            course.MaxCapacity, enrollmentCount);

        var links = new List<LinkDto>
        {
            new(linkGenerator.GetPathByName(HttpContext, nameof(GetCourseById), new { id })!, "self", "GET"),
            new($"/api/courses/{id}/enrollments", "enrollments", "GET")
        };

        if (enrollmentCount < course.MaxCapacity)
        {
            links.Add(new LinkDto($"/api/courses/{id}/enrollments", "enroll", "POST"));
        }

        var detail = new CourseDetailDto(
            dto.Id, dto.Code, dto.Title, dto.MaxCapacity, dto.EnrollmentCount, links);

        return Ok(detail);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse(
        CreateCourseRequest request,
        CancellationToken ct)
    {
        if (await courseService.CodeExistsAsync(request.Code, ct))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course code already exists",
                Detail = $"A course with code '{request.Code}' is already registered.",
                Status = StatusCodes.Status409Conflict
            });
        }

        // Build the Course entity from the DTO
        var course = new Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        var created = await courseService.CreateAsync(course, ct);

        var dto = new CourseResponseDto(
            created.Id, created.Code, created.Title, created.MaxCapacity, 0);

        return CreatedAtAction(nameof(GetCourseById), new { id = dto.Id }, dto);
    }
}
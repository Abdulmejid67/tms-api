using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Dtos;
using TmsApi.Application.Interfaces;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/courses")]
[Tags("Courses (Legacy)")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] PagedRequest request,
        CancellationToken ct)
    {
        var result = await courseService.GetCoursesAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(id, ct);
        if (course is null) return NotFound();

        var dto = new CourseResponseDto(
            course.Id, course.Code, course.Title,
            course.MaxCapacity, course.Enrollments?.Count ?? 0);

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourseRequest request,
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

        var course = new TmsApi.Domain.Entities.Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        var created = await courseService.CreateAsync(course, ct);
        var dto = new CourseResponseDto(
            created.Id, created.Code, created.Title, created.MaxCapacity, 0);

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
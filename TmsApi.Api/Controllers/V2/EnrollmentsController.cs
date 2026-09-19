using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Common;
using TmsApi.Application.Enrollments.Commands;
using TmsApi.Application.Enrollments.Queries;

namespace TmsApi.Api.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/enrollments")]
[ApiVersion("2.0")]
[Tags("Enrollments V2")]
public class EnrollmentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Enroll(
        [FromBody] EnrollStudentCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.Match<IActionResult>(
            onSuccess: created => CreatedAtAction(
                nameof(GetSchedule),
                new { studentId = created.StudentId },
                created),
            onFailure: error => error.Code switch
            {
                "course_not_found" => Problem(
                    title: "Course not found",
                    detail: error.Message,
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://tms.local/errors/course_not_found"),
                "course_full" => Problem(
                    title: "Course is full",
                    detail: error.Message,
                    statusCode: StatusCodes.Status409Conflict,
                    type: "https://tms.local/errors/course_full"),
                "already_enrolled" => Problem(
                    title: "Already enrolled",
                    detail: error.Message,
                    statusCode: StatusCodes.Status409Conflict,
                    type: "https://tms.local/errors/already_enrolled"),
                _ => Problem(
                    title: "Unknown error",
                    detail: error.Message,
                    statusCode: StatusCodes.Status400BadRequest)
            });
    }

    [HttpGet("{studentId:int}/schedule")]
    public async Task<IActionResult> GetSchedule(int studentId, CancellationToken ct)
    {
        var schedule = await mediator.Send(new GetStudentScheduleQuery(studentId), ct);
        return Ok(schedule);
    }
}
using MediatR;
using TmsApi.Application.Interfaces;

namespace TmsApi.Application.Enrollments.Queries;

public class GetStudentScheduleHandler(IEnrollmentService enrollmentService)
    : IRequestHandler<GetStudentScheduleQuery, ScheduleDto>
{
    public async Task<ScheduleDto> Handle(GetStudentScheduleQuery request, CancellationToken ct)
    {
        var enrollments = await enrollmentService.GetByStudentIdAsync(request.StudentId, ct);

        var items = enrollments
            .Where(e => e.Course is not null)
            .Select(e => new ScheduleItemDto(
                e.Course.Code,
                e.Course.Title,
                $"Enrolled at {e.EnrolledAt:yyyy-MM-dd}"))
            .ToList();

        return new ScheduleDto(request.StudentId, items);
    }
}
namespace TmsApi.Application.Dtos;

public record CourseDetailDto(
    int Id,
    string Code,
    string Title,
    int MaxCapacity,
    int EnrollmentCount,
    IReadOnlyList<LinkDto> Links
);
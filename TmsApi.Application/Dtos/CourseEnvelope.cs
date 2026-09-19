namespace TmsApi.Application.Dtos;

public record CourseEnvelope(
    IReadOnlyList<CourseResponseDto> Data,
    CourseEnvelopeMeta Meta,
    CourseEnvelopeLinks Links);

public record CourseEnvelopeMeta(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);

public record CourseEnvelopeLinks(
    string Self,
    string? Next,
    string? Prev,
    string Enroll);
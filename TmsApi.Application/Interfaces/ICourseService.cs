using TmsApi.Application.Dtos;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface ICourseService
{
    // Single-item operations
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);
    Task<Course?> GetByCodeAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct);

    // Write operations
    Task<Course> CreateAsync(Course course, CancellationToken ct);
    Task UpdateAsync(Course course, CancellationToken ct);

    // Existence checks
    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<bool> ExistsAsync(int id, CancellationToken ct);

    // Paginated query (M6/M7)
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct);
}
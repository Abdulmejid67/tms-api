using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface ICourseService
{
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);
    Task<Course?> GetByCodeAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct);
    Task<Course> CreateAsync(Course course, CancellationToken ct);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<bool> ExistsAsync(int id, CancellationToken ct);
    Task UpdateAsync(Course course, CancellationToken ct);
}
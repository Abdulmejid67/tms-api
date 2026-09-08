using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface IEnrollmentService
{
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct);
    Task<IReadOnlyList<Enrollment>> GetByCourseIdAsync(int courseId, CancellationToken ct);
    Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct);
    Task AddAsync(Enrollment enrollment, CancellationToken ct);
}
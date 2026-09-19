using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface IEnrollmentService
{
    // ---- Legacy (M4/M5) ----
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct);
    Task<IReadOnlyList<Enrollment>> GetByCourseIdAsync(int courseId, CancellationToken ct);
    Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct);
    Task AddAsync(Enrollment enrollment, CancellationToken ct);

    // ---- M6/M7 nested + DTO ----
    Task<Enrollment?> GetByIdAsync(int courseId, int id, CancellationToken ct);
    Task<IReadOnlyList<Enrollment>> GetByCourseAsync(int courseId, CancellationToken ct);
    Task<Enrollment> CreateAsync(int courseId, int studentId, CancellationToken ct);
}
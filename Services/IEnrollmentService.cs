using TmsApi.Dtos;
using TmsApi.Entities;

namespace TmsApi.Services;

public interface IEnrollmentService
{
    // ========== M4/M5 METHODS ==========
    Task<Enrollment> EnrollAsync(EnrollmentRequest request);
    Task<Enrollment?> GetByIdAsync(int id);
    Task<IReadOnlyList<Enrollment>> GetAllAsync();
    Task<bool> DeleteAsync(int id);

    // ========== M6 METHODS ==========
    Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct);
    Task<IReadOnlyList<EnrollmentResponseDto>> GetByCourseAsync(int courseId, CancellationToken ct);
    Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct);
}
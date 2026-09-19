using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly TmsDbContext _context;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(TmsDbContext context, ILogger<EnrollmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ---- Legacy (M4/M5) ----
    public async Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByCourseIdAsync(int courseId, CancellationToken ct)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.CourseId == courseId)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .AnyAsync(e => e.StudentId == studentId && e.Course.Code == courseCode, ct);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Enrolled student {StudentId} in course {CourseId}",
            enrollment.StudentId, enrollment.CourseId);
    }

    // ---- M6/M7 nested / DTO ----
    public async Task<Enrollment?> GetByIdAsync(int courseId, int id, CancellationToken ct)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.Id == id && e.CourseId == courseId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByCourseAsync(int courseId, CancellationToken ct)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Where(e => e.CourseId == courseId)
            .ToListAsync(ct);
    }

    public async Task<Enrollment> CreateAsync(int courseId, int studentId, CancellationToken ct)
    {
        var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = studentId,
            EnrolledAt = DateTime.UtcNow
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Enrolled student {StudentId} in course {CourseId}",
            studentId, courseId);

        return enrollment;
    }

    
}
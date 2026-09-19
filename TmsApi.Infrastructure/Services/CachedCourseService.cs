using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Dtos;
using TmsApi.Application.Interfaces;
using TmsApi.Infrastructure.Caching;

namespace TmsApi.Infrastructure.Services;

public class CachedCourseService(
    HybridCache cache,
    ICourseService courseService,
    ILogger<CachedCourseService> logger)
    : ICachedCourseService
{
    public async Task<CourseResponseDto> GetCourseByCodeAsync(string code, CancellationToken ct)
    {
        var key = CacheKeys.CourseByCode(code);
        var dbHit = false;

        var dto = await cache.GetOrCreateAsync(
            key,
            (service: courseService, code),
            async (state, token) =>
            {
                dbHit = true;
                logger.LogInformation("Cache MISS for {Key} fetching from DB", key);
                var course = await state.service.GetByCodeAsync(state.code, token)
                    ?? throw new InvalidOperationException($"Course {state.code} not found.");

                return new CourseResponseDto(
                    course.Id, course.Code, course.Title,
                    course.MaxCapacity, course.Enrollments.Count);
            },
            tags: [CacheKeys.CoursesTag],
            cancellationToken: ct);

        if (!dbHit)
            logger.LogInformation("Cache HIT for {Key}", key);

        return dto;
    }

    public async Task<IReadOnlyList<CourseResponseDto>> GetAllCoursesAsync(CancellationToken ct)
    {
        var key = CacheKeys.CoursesAll;
        var dbHit = false;

        var list = await cache.GetOrCreateAsync(
            key,
            courseService,
            async (svc, token) =>
            {
                dbHit = true;
                logger.LogInformation("Cache MISS for {Key} fetching from DB", key);
                var courses = await svc.GetAllAsync(token);
                return courses
                    .Select(c => new CourseResponseDto(
                        c.Id, c.Code, c.Title, c.MaxCapacity, c.Enrollments.Count))
                    .ToList();
            },
            tags: [CacheKeys.CoursesTag],
            cancellationToken: ct);

        if (!dbHit)
            logger.LogInformation("Cache HIT for {Key}", key);

        return list;
    }

    public async Task InvalidateCourseCacheAsync(CancellationToken ct)
    {
        logger.LogInformation("Invalidating cache tag {Tag}", CacheKeys.CoursesTag);
        await cache.RemoveByTagAsync(CacheKeys.CoursesTag, ct);
    }
}
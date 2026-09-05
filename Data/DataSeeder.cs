using Microsoft.EntityFrameworkCore;
using TmsApi.Entities;

namespace TmsApi.Data;

public static class DataSeeder
{
    private static readonly (string Code, string Title, int MaxCapacity)[] Courses =
    [
        ("CSE-101", "Web Development Fundamentals", 30),
        ("CSE-102", "TypeScript Essentials", 30),
        ("CSE-103", "Git and Collaborative Workflows", 25),
        ("CSE-201", "ASP.NET Core Fundamentals", 28),
        ("CSE-202", "Entity Framework Core and PostgreSQL", 28),
        ("CSE-203", "Building RESTful Web APIs", 28),
        ("CSE-301", "Advanced Web API Patterns", 24),
        ("CSE-302", "Angular Fundamentals", 26),
        ("CSE-303", "Angular Advanced", 24),
        ("CSE-304", "Full-Stack Integration", 22),
        ("CSE-305", "Testing and Quality Assurance", 22),
        ("CSE-306", "Security and Authentication", 20),
        ("CSE-307", "Cloud Deployment and DevOps", 20),
        ("CSE-308", "Database Design and Optimization", 24),
        ("CSE-309", "Microservices Architecture", 18),
        ("CSE-401", "Project Management for Developers", 25),
        ("CSE-402", "Technical Leadership", 20),
        ("CSE-403", "System Design and Architecture", 24),
        ("CSE-404", "Machine Learning Fundamentals", 22),
        ("CSE-405", "Mobile Application Development", 26),
        ("CSE-406", "Cybersecurity Essentials", 20),
        ("CSE-407", "Blockchain Fundamentals", 18),
        ("CSE-408", "Data Analytics and Visualization", 24),
        ("CSE-409", "Internet of Things (IoT)", 20),
        ("CSE-410", "Ethical Hacking and Penetration Testing", 16)
    ];

    public static async Task SeedAsync(TmsDbContext context, CancellationToken ct = default)
    {
        await context.Database.MigrateAsync(ct);

        if (await context.Courses.AnyAsync(ct))
        {
            return;
        }

        foreach (var (code, title, maxCapacity) in Courses)
        {
            context.Courses.Add(new Course
            {
                Code = code,
                Title = title,
                MaxCapacity = maxCapacity
            });
        }

        await context.SaveChangesAsync(ct);
    }
}
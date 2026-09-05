using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Entities;

namespace TmsApi.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Grade)
            .HasPrecision(3, 2);

        builder.Property(e => e.EnrolledAt)
            .IsRequired();

        // Relationships with explicit OnDelete behavior (Session 2)
        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete students

        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete courses

        // Unique constraint to prevent duplicate enrollments
        builder.HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique();
    }
}
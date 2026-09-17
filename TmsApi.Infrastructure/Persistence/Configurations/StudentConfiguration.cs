using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Domain.Entities;


namespace TmsApi.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.GPA)
            .HasPrecision(3, 2);

        // Unique constraint on natural key
        builder.HasIndex(s => s.RegistrationNumber)
            .IsUnique();

        // Soft delete filter (Session 3)
        builder.HasQueryFilter(s => !s.IsDeleted);

        // Concurrency token (Session 3)
        builder.Property(s => s.Version)
            .IsRowVersion();
    }
}
using AulasECursos.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AulasECursos.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureCourse(builder);
            ConfigureStudent(builder);
            ConfigureEnrollment(builder);
        }

        private static void ConfigureCourse(ModelBuilder builder)
        {
            builder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses", t => 
                {
                    t.HasCheckConstraint("CK_Courses_WorkloadHours_Positive", "WorkloadHours > 0");
                });

                entity.Property(c => c.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.Category)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.WorkloadHours)
                      .IsRequired();

                entity.HasIndex(c => c.Category)
                      .HasDatabaseName("IX_Courses_Category")
                      .HasFilter("[IsDeleted] = 0");
            });
        }

        private static void ConfigureStudent(ModelBuilder builder)
        {
            builder.Entity<Student>(entity =>
            {
                entity.ToTable("Students", t => 
                {
                    t.HasCheckConstraint("CK_Students_Email_Format", "Email LIKE '_%@_%._%'");
                });

                entity.Property(s => s.UserId)
                      .IsRequired()
                      .HasMaxLength(450);

                entity.Property(s => s.FullName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(s => s.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.HasIndex(s => s.UserId)
                      .IsUnique()
                      .HasDatabaseName("UX_Students_UserId")
                      .HasFilter("[IsDeleted] = 0");

                entity.HasIndex(s => s.Email)
                      .IsUnique()
                      .HasDatabaseName("UX_Students_Email")
                      .HasFilter("[IsDeleted] = 0");

                entity.HasOne<IdentityUser>()
                      .WithOne()
                      .HasForeignKey<Student>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureEnrollment(ModelBuilder builder)
        {
            builder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollments");
                entity.Property(e => e.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => new { e.StudentId, e.CourseId })
                      .IsUnique()
                      .HasDatabaseName("UX_Enrollments_Student_Course_Active")
                      .HasFilter("[Status] = 'Active'");

                entity.HasIndex(e => e.StudentId)
                      .HasDatabaseName("IX_Enrollments_StudentId");
            });
        }
    }
}

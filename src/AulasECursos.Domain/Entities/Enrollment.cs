namespace AulasECursos.Domain.Entities
{
    public enum EnrollmentStatus
    {
        Active,
        Cancelled
    }

    public class Enrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    }

}

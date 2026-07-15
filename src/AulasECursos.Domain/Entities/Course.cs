namespace AulasECursos.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int WorkloadHours { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }

        // Navegação: um curso tem N matrículas.
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}

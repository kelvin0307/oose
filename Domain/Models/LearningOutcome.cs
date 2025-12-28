namespace Domain.Models
{
    public class LearningOutcome
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimumLessons { get; set; }

        public Course? Course { get; set; }
        public int? CourseId { get; set; }
        public List<Lesson>? Lessons { get; set; }
    }
}

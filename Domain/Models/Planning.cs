namespace Domain.Models
{
    public class Planning
    {
        public int Id { get; set; }

        public Course? Course { get; set; }
        public int? CourseId { get; set; }
        public List<Lesson>? Lessons { get; set; }
    }
}

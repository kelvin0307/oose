namespace Domain.Models;

public class Material
{
    public int Id { get; set; }
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public IList<CourseExecution>? CourseExecutions { get; set; }
    public DateTimeOffset? SysDeleted { get; set; }


}

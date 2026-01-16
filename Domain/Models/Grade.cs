namespace Domain.Models;

public class Grade
{
    public int Id { get; set; }
    public int CourseExcecutionId { get; set; }
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public Student? Student { get; set; }
    public Lesson? Lesson { get; set; }
    public CourseExecution? CourseExecution { get; set; }
}

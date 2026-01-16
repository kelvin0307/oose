namespace Domain.Models;

public class Grade
{
    public int Id { get; set; }
    public int GradeValue { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public int CourseExcecutionId { get; set; }
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public Student? Student { get; set; }
    public Lesson? Lesson { get; set; }
    public CourseExecution? CourseExecution { get; set; }
}

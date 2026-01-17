namespace Core.DTOs;

public class CreateGradeDTO
{
    public string Grade { get; set; } = string.Empty; // numeric string or letter
    public string? Feedback { get; set; }
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public int CourseExecutionId { get; set; }
}

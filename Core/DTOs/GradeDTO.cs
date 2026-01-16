namespace Core.DTOs;

public class GradeDTO
{
    public int Id { get; set; }
    public int GradeValue { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public int CourseExecutionId { get; set; }
    public string StudentFirstName { get; set; } = string.Empty;
    public string StudentLastName { get; set; } = string.Empty;
}

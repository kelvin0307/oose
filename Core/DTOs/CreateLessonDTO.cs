namespace Core.DTOs;

public class CreateLessonDto
{
    public int WeekNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public int PlanningId { get; set; }
    public int? TestType { get; set; }
    public int? Weight { get; set; }
}

using Domain.Models;

namespace Core.DTOs;

public class LessonDTO
{
    public int Id { get; set; }
    public int WeekNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public List<LearningOutcome>? LearningOutcomes { get; set; }
}

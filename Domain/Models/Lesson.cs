using Domain.Enums;

namespace Domain.Models;

public class Lesson
{
    public int Id { get; set; }
    public int WeekNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public int PlanningId { get; set; }
    public Planning? Planning { get; set; }
    public IList<Material>? Materials { get; set; } = [];
    public IList<LearningOutcome> LearningOutcomes { get; set; } = [];

    //test specific
    public TestType? TestType { get; set; }
    public int? Weight { get; set; }
    public IList<Grade> Grades { get; set; } = [];
}

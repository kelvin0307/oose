using Domain.Enums;

namespace Domain.Models;
public class Course
{
    public int Id { get; set; }
    public double EuropeanCredits { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public TimeSpan Duration { get; set; }
    public CourseStatus Status { get; set; }

    public Planning? Planning { get; set; }
    public ICollection<LearningOutcome>? LearningOutcomes { get; set; } = new List<LearningOutcome>();
}

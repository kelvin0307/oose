namespace Domain.Models;

public class Rubric
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public LearningOutcome LearningOutcome { get; set; }
    public int LearningOutcomeId { get; set; }
    public ICollection<AssessmentDimension> AssessmentDimensions { get; set; } = new List<AssessmentDimension>();
}
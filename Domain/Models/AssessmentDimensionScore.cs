namespace Domain.Models;

public class AssessmentDimensionScore
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string Description { get; set; }
    
    public AssessmentDimension? AssessmentDimension { get; set; }
}
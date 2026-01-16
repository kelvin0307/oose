namespace Domain.Models;

public class AssessmentDimension
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameCriterium { get; set; }
    public int Wage { get; set; }
    public int MinimumScore { get; set; }
    
    public Rubric? Rubric { get; set; }
    public int RubricId { get; set; }
    public ICollection<AssessmentDimensionScore> AssessmentDimensionScores { get; set; } = new List<AssessmentDimensionScore>();
}
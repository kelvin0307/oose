namespace Core.DTOs;

public class AssessmentDimensionDto
{
    public int Id { get; set; }
    public int RubricId { get; set; }
    public string Name { get; set; }
    public string NameCriterium { get; set; }
    public int Wage { get; set; }
    public int MinimumScore { get; set; }
    public ICollection<AssessmentDimensionScoreDto> AssessmentDimensionScores { get; set; } = new List<AssessmentDimensionScoreDto>();
}
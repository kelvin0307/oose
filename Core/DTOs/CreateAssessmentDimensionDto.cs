namespace Core.DTOs;

public class CreateAssessmentDimensionDto
{
    public string Name { get; set; }
    public string NameCriterium { get; set; }
    public int Wage { get; set; }
    public int MinimumScore { get; set; }
    public ICollection<CreateAssessmentDimensionScoreDto> AssessmentDimensionScores { get; set; } = new List<CreateAssessmentDimensionScoreDto>();

}
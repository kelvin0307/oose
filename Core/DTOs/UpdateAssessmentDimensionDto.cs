namespace Core.DTOs;

public class UpdateAssessmentDimensionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameCriterium { get; set; }
    public int Wage { get; set; }
    public int MinimumScore { get; set; }
    public ICollection<UpdateAssessmentDimensionScoreDto> AssessmentDimensionScores { get; set; } = new List<UpdateAssessmentDimensionScoreDto>();

}
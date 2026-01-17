namespace Core.DTOs.Imports.Nijmegen;

public class NijmegenAssessmentDimensionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameCriterium { get; set; }
    public int Wage { get; set; }
    public int MinimumScore { get; set; }
    public ICollection<NijmegenAssessmentDimensionScoreDto> AssessmentDimensionScores { get; set; } = new List<NijmegenAssessmentDimensionScoreDto>();   
}
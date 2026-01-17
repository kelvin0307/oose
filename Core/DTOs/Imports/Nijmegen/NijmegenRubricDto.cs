namespace Core.DTOs.Imports.Nijmegen;

public class NijmegenRubricDto
{
    public int Id { get; set; }
    public int LearningOutcomeId { get; set; }
    public string Name { get; set; }
    public ICollection<NijmegenAssessmentDimensionDto> AssessmentDimensions { get; set; } = new List<NijmegenAssessmentDimensionDto>();
}
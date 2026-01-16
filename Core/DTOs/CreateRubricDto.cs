namespace Core.DTOs;

public class CreateRubricDto
{
    public int LearningOutcomeId { get; set; }
    public string Name { get; set; }
    public ICollection<CreateAssessmentDimensionDto> AssessmentDimensions { get; set; } = new List<CreateAssessmentDimensionDto>();
}
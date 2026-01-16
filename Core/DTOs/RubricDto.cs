namespace Core.DTOs;

public class RubricDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LearningOutcomeId { get; set; }
    public ICollection<AssessmentDimensionDto> AssessmentDimensions { get; set; } = new List<AssessmentDimensionDto>();
}
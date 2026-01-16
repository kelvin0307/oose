namespace Core.DTOs;

public class UpdateRubricDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<UpdateAssessmentDimensionDto> AssessmentDimensions { get; set; } = new List<UpdateAssessmentDimensionDto>();
}
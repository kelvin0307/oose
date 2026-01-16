namespace Core.DTOs;

public class AssessmentDimensionScoreDto
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string Description { get; set; }
    public int AssessmentDimensionId { get; set; }
}
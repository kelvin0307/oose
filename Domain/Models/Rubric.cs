namespace Domain.Models;

public class Rubric
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<AssessmentDimension>? AssessmentDimensions { get; set; }
}
namespace Core.DTOs;

public class CreateLearningOutcomeDto
{
    public int CourseId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string EndQualification { get; set; }
}
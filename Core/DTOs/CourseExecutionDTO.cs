namespace Core.DTOs;

public class CourseExecutionDto
{
    public int Id { get; set; }
    public CourseDto? Course { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ClassDto>? Classes { get; set; }

    public List<MaterialDto>? Materials { get; set; }
}

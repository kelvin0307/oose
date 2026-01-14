namespace Core.DTOs;

public class CourseExecutionDTO
{
    public int Id { get; set; }
    public CourseDTO? Course { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ClassDTO>? Classes { get; set; }

    public List<MaterialDTO>? Materials { get; set; }
}

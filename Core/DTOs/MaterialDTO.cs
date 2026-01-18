namespace Core.DTOs;

public class MaterialDto
{
    public int Id { get; set; }
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public IList<CourseExecutionDto>? CourseExecutions { get; set; }
}

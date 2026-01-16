namespace Core.DTOs;

public class MaterialDTO
{
    public int Id { get; set; }
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public IList<CourseExecutionDTO>? CourseExecutions { get; set; }
}

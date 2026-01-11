namespace Domain.Models;

public class Material
{
    public int Id { get; set; }
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public IList<CourseExecution>? CourseExecutions { get; set; }
}

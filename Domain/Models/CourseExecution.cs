namespace Domain.Models;

public class CourseExecution
{
    public int Id { get; set; }
    public Course? Course { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Class>? Classes { get; set; }
    public List<Material>? Materials { get; set; }
}

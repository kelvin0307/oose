namespace Core.DTOs;
public class PlanningDTO
{
    public int Id { get; set; }
    public IList<LessonDTO>? Lessons { get; set; } = [];
}
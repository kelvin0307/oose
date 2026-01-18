namespace Core.DTOs;
public class PlanningDto
{
    public int Id { get; set; }
    public IList<LessonDto> Lessons { get; set; } = [];
}
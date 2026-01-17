namespace Core.DTOs.Imports.Nijmegen;

public class NijmegenPlanningDto
{
    public int Id { get; set; }
    public int SysCode { get; set; }

    public NijmegenCourseDto? Course { get; set; }
    public int? CourseId { get; set; }
    public ICollection<NijmegenLessonDto>? Lessons { get; set; } =  new List<NijmegenLessonDto>();
}
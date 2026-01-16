namespace Data.Adapters.Nijmegen.DTOs;

public class NijmegenPlanningDto
{
    public int SysCode { get; set; }

    public NijmegenCourseDto? Course { get; set; }
    public int? CourseId { get; set; }
    public List<NijmegenLessonDto>? Lessons { get; set; }
}

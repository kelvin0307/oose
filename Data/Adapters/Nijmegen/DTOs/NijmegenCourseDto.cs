namespace Data.Adapters.Nijmegen.DTOs;

public class NijmegenCourseDto
{
    public int SysCode { get; set; }
    public string? Naam { get; set; }
    public string? Beschrijving { get; set; }
    public int Status { get; set; }

    public NijmegenPlanningDto? Planning { get; set; }


    public List<NijmegenLearningOutcomeDto> Leeruitkomsten { get; set; } = new();
}

using Core.DTOs.Imports.Nijmegen;

namespace Core.DTOs.Imports.Nijmegen;

public class NijmegenImportDataDto
{
    public NijmegenCourseDto Course { get; set; }
    public ICollection<NijmegenLearningOutcomeDto> LearningOutcomes { get; set; } = new List<NijmegenLearningOutcomeDto>();
    public ICollection<NijmegenPlanningDto> Planning { get; set; } = new List<NijmegenPlanningDto>();
    public ICollection<NijmegenRubricDto> Rubrics { get; set; } = new List<NijmegenRubricDto>();
    
}
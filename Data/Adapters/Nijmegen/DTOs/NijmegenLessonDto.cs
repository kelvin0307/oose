using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Adapters.Nijmegen.DTOs;

public class NijmegenLessonDto
{
    public int SysCode { get; set; }
    public int Weeknummer { get; set; }
    public string Naam { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public TestType? TestVariant { get; set; }

    public NijmegenPlanningDto? Planning { get; set; }
    public int? PlanningId { get; set; }

    public List<NijmegenLearningOutcomeDto> Leeruitkomsten { get; set; } = new();
}

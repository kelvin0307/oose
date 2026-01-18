using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Imports.Nijmegen;

public class NijmegenLessonDto
{
    public int Id { get; set; }
    public int SysCode { get; set; }
    public int Weeknummer { get; set; }
    public string Naam { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public TestType? TestVariant { get; set; }
    public int? PlanningId { get; set; }
    public ICollection<int> LearningOutcomeIds { get; set; } = new List<int>();
}
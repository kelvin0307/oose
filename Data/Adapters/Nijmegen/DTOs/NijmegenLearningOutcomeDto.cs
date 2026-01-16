using Core.DTOs;
using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Adapters.Nijmegen.DTOs;

public class NijmegenLearningOutcomeDto
{
    public int SysCode { get; set; }
    public string Naam { get; set; }
    public string Beschrijving { get; set; }
    public string Eindkwalificatie { get; set; }

    public NijmegenCourseDto Course { get; set; }
    public int CourseId { get; set; }

    public List<NijmegenLessonDto> Lessons { get; set; } = new();
}

using Core.DTOs;
using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Imports.Nijmegen;

public class NijmegenLearningOutcomeDto
{
    public int Id { get; set; }
    public int SysCode { get; set; }
    public string Naam { get; set; }
    public string Beschrijving { get; set; }
    public string Eindkwalificatie { get; set; }
    public int CourseId { get; set; }
}
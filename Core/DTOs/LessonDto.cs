using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class LessonDto
    {
        public int Id { get; set; }
        public int WeekNumber { get; set; }
        public string Name { get; set; }
        public int SequenceNumber { get; set; }
        public TestType? TestType { get; set; }

        public int? PlanningId { get; set; }
    }
}

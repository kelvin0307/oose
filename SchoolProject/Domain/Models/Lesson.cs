using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public int WeekNumber { get; set; }
        public string Name { get; set; }
        public int SequenceNumber { get; set; }

        public Planning Planning { get; set; }
        public int PlanningId { get; set; }
        public Test? Test { get; set; }
        public int? TestId { get; set; }

        public ICollection<LearningOutcome> LearningOutcomes { get; set; } = new List<LearningOutcome>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Planning Planning { get; set; }
        public ICollection<LearningOutcome> LearningOutcomes { get; set; } = new List<LearningOutcome>();
    }
}

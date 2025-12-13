using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class LearningOutcome
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimumLessons { get; set; }

        public Course Course { get; set; }
        public int CourseId { get; set; }
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}

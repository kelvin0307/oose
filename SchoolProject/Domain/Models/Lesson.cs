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

        public ICollection<LearningOutcome> LearningOutcomes { get; set; } = new List<LearningOutcome>();
    }
}

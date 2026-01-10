using Domain.Models;
using System.Linq;

namespace Core.Extensions;

public static class LearningOutcomeExtensions
{
    public static IQueryable<LearningOutcome> GetLearningOutcomesByCourseId(this IQueryable<LearningOutcome> learningOutcomes, int courseId)
    {
        return learningOutcomes.Where(learningOutcome => learningOutcome.CourseId == courseId); //TODO: add any
    }
}
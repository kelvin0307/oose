using Domain.Models;

namespace Core.Extensions;

public static class LessonExtension
{
    public static IQueryable<Lesson> FindLessonsByLearningOutcomeId(this IQueryable<Lesson> lessons, int learningOutcomeId)
    {
        return lessons.Where(lesson => lesson.LearningOutcomes != null
                                        && lesson.TestType == null
                                        && lesson.LearningOutcomes.Any(lo => lo.Id == learningOutcomeId));
    }

    public static IQueryable<Lesson> FindTestsLearningOutcomeId(this IQueryable<Lesson> lessons, int learningOutcomeId)
    {
        return lessons.Where(lesson => lesson.LearningOutcomes != null
                                        && lesson.TestType != null
                                        && lesson.LearningOutcomes.Any(lo => lo.Id == learningOutcomeId));
    }

    public static IQueryable<Lesson> FindLessonsAndTestsByLearningOutcomeId(this IQueryable<Lesson> lessons, int learningOutcomeId)
    {
        return lessons.Where(lesson => lesson.LearningOutcomes != null
                                        && lesson.LearningOutcomes.Any(lo => lo.Id == learningOutcomeId));
    }
}


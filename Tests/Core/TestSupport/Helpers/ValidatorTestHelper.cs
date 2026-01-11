using Domain.Models;
using Domain.Enums;

namespace Tests.Core.TestSupport.Helpers;

public static class ValidatorTestHelpers
{
    public static Lesson CreateLesson(int week, int sequence, TestType? testType = null)
    {
        return new Lesson
        {
            WeekNumber = week,
            SequenceNumber = sequence,
            TestType = testType
        };
    }

    public static List<LearningOutcome> SetupLearningOutcomes(params LearningOutcome[] outcomes)
    {
        return new List<LearningOutcome>(outcomes);
    }
}

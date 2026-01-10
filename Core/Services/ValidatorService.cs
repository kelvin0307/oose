using Core.Common;
using Core.Extentions.ModelExtensions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class ValidatorService(
    IRepository<LearningOutcome> learningOutcomeRepository
    ) : IValidatorService
{
    private readonly int MINIMUM_LESSOSN = 1;

    public Response<string> ValidateCoursePlanning(int courseId)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var learningOutcomesAndLessons = learningOutcomeRepository.Include(x => x.Lessons)
            .FindByCourseId(courseId)
            .ToList();

        if (!learningOutcomesAndLessons.Any())
        {
            validationErrors.Add(
                "Course",
                [$"No learning outcomes and lessons found for course {courseId}."]
            );

            return Response<string>.ValidationFail(validationErrors);
        }

        foreach (var learningOutcome in learningOutcomesAndLessons)
        {
            var errors = new List<string>();

            if (learningOutcome.Lessons.Count() < MINIMUM_LESSOSN)
            {
                errors.Add($"Learning outcome '{learningOutcome.Name}' has no lessons.");
            }

            var lastLesson = learningOutcome.Lessons
                                            .OrderByDescending(l => l.WeekNumber)
                                            .ThenByDescending(l => l.SequenceNumber)
                                            .FirstOrDefault();

            if (lastLesson == null || lastLesson.TestType == null)
            {
                errors.Add($"The last lesson of learning outcome '{learningOutcome.Name}' is not a test.");
            }

            if (errors.Any())
            {
                validationErrors.Add($"LearningOutcome_{learningOutcome.Id}", errors.ToArray());
            }
        }

        return validationErrors.Any()
            ? Response<string>.ValidationFail(validationErrors)
            : Response<string>.Ok("Course planning is valid.");
    }
}
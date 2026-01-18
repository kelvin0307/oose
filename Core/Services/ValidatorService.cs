using Core.Common;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;

public class ValidatorService(
    IRepository<LearningOutcome> learningOutcomeRepository,
    IRubricRepository rubricRepository,
    IRepository<Planning> planningRepository,
    IRepository<Course> courseRepository
) : IValidatorService
{
    private const int MINIMUM_LESSONS = 1;

    public async Task<Response<string>> ValidateCoursePlanning(int courseId)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var planningWithLessons = planningRepository
            .Include(x => x.Lessons)
            .Where(x => x.CourseId == courseId)
            .FirstOrDefault();

        if (planningWithLessons == null)
        {
            validationErrors.Add(
                "Planning",
                new[] { $"No planning found for course {courseId}." }
            );
        }

        var learningOutcomesWithLessons = learningOutcomeRepository
            .Include(x => x.Lessons)
            .Where(x => x.CourseId == courseId)
            .ToList();

        if (!learningOutcomesWithLessons.Any())
        {
            validationErrors.Add(
                "Course",
                new[] { $"No learning outcomes and lessons found for course {courseId}." }
            );

            return Response<string>.ValidationFail(validationErrors);
        }

        foreach (var learningOutcome in learningOutcomesWithLessons)
        {
            var learningOutcomeErrors = new List<string>();

            if (learningOutcome.Lessons.Count < MINIMUM_LESSONS)
            {
                learningOutcomeErrors.Add(
                    $"Learning outcome '{learningOutcome.Name}' has no lessons."
                );
            }

            var lastLesson = learningOutcome.Lessons
                .OrderByDescending(l => l.WeekNumber)
                .ThenByDescending(l => l.SequenceNumber)
                .FirstOrDefault();

            if (lastLesson == null || lastLesson.TestType == null)
            {
                learningOutcomeErrors.Add(
                    $"The last lesson of learning outcome '{learningOutcome.Name}' is not a test."
                );
            }

            if (learningOutcomeErrors.Any())
            {
                validationErrors.Add(
                    $"LearningOutcome_{learningOutcome.Id}",
                    learningOutcomeErrors.ToArray()
                );
            }

            var rubrics = await rubricRepository
                .GetAggregatesByLearningOutcomeId(learningOutcome.Id);

            if (rubrics == null || !rubrics.Any())
            {
                validationErrors.Add(
                    $"LearningOutcome_{learningOutcome.Id}_Rubric",
                    new[] { $"Learning outcome '{learningOutcome.Name}' has no rubrics." }
                );
                continue;
            }

            foreach (var rubric in rubrics)
            {
                var rubricErrors = new List<string>();

                if (!rubric.AssessmentDimensions.Any())
                {
                    rubricErrors.Add(
                        $"Rubric '{rubric.Name}' has no assessment dimensions."
                    );
                }
                else
                {
                    foreach (var dimension in rubric.AssessmentDimensions)
                    {
                        if (dimension.AssessmentDimensionScores.Count < 2)
                        {
                            rubricErrors.Add(
                                $"Assessment dimension '{dimension.Name}' has fewer than 2 scores."
                            );
                        }
                    }
                }

                if (rubricErrors.Any())
                {
                    validationErrors.Add(
                        $"Rubric_{rubric.Id}",
                        rubricErrors.ToArray()
                    );
                }
            }
        }

        if (planningWithLessons?.Lessons != null)
        {
            foreach (var lesson in planningWithLessons.Lessons)
            {
                var invalidLearningOutcomes = lesson.LearningOutcomes
                    .Where(lo =>
                        !learningOutcomesWithLessons.Any(courseLo => courseLo.Id == lo.Id)
                    )
                    .ToList();

                if (invalidLearningOutcomes.Any())
                {
                    validationErrors.Add(
                        $"Lesson_{lesson.Id}",
                        invalidLearningOutcomes.Select(lo =>
                            $"Lesson '{lesson.Name}' references LearningOutcome '{lo.Name}' which does not belong to course {courseId}."
                        ).ToArray()
                    );
                }
            }
        }

        if (!validationErrors.Any())
        {
            var course = courseRepository
                .Where(x => x.Id == courseId)
                .FirstOrDefault();

            if (course != null)
            {
                course.Status = CourseStatus.Validated;
                await courseRepository.UpdateAndCommit(course);
            }
        }

        return validationErrors.Any()
            ? Response<string>.ValidationFail(validationErrors)
            : Response<string>.Ok("Course planning is valid.");
    }
}

using Core.Common;
using Core.Extensions;
using Core.Extentions.ModelExtentions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services
{
    public class ValidatorService(IRepository<LearningOutcome> learningOutcomeRepository,
        IRepository<Lesson> lessonRepository) : IValidatorService
    {
        public Task<Response<string>> ValidateCoursePlanning(int courseId)
        {
            var validationErrors = new Dictionary<string, string[]>();

            var learningOutcomes = learningOutcomeRepository
                .FindByCourseId(courseId)
                .ToList();

            if (!learningOutcomes.Any())
            {
                validationErrors.Add(
                    "Course",
                    [$"No learning outcomes found for course {courseId}."]
                );
            }

            foreach (var learningOutcome in learningOutcomes)
            {
                var errors = new List<string>();

                var lessons = lessonRepository
                    .FindLessonsByLearningOutcomeId(learningOutcome.Id)
                    .ToList();

                if (!lessons.Any())
                {
                    errors.Add(
                        $"Learning outcome '{learningOutcome.Name}' has no lessons."
                    );
                }

                var lastLesson = lessonRepository
                    .FindLessonsAndTestsByLearningOutcomeId(learningOutcome.Id)
                    .OrderByDescending(x => x.WeekNumber)
                    .ThenByDescending(x => x.SequenceNumber)
                    .FirstOrDefault();

                if (lastLesson == null || lastLesson.TestType == null)
                {
                    errors.Add(
                        $"The last lesson of learning outcome '{learningOutcome.Name}' is not a test."
                    );
                }

                if (errors.Any())
                {
                    validationErrors.Add(
                        $"LearningOutcome_{learningOutcome.Id}",
                        errors.ToArray()
                    );
                }
            }

            if (validationErrors.Any())
            {
                return Task.FromResult(
                    Response<string>.ValidationFail(validationErrors)
                );
            }

            return Task.FromResult(
                Response<string>.Ok("Course planning is valid.")
            );
        }


    }
}

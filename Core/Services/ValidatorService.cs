using Core.Common;
using Core.Extensions;
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
            var response = new Response<string>();

            var learningOutcomes = learningOutcomeRepository.FindByCourseId(courseId);

            foreach (var learningOutcome in learningOutcomes)
            {
                var lessons = lessonRepository.FindLessonsByLearningOutcomeId(learningOutcome.Id);
                if (lessons.Count() < 1)
                {
                    response.Success = false;
                    response.Message = $"Learning outcome '{learningOutcome.Name}' had not enough lessons.";
                }

                var lastLesson = lessonRepository
                    .FindLessonsAndTestsByLearningOutcomeId(learningOutcome.Id)
                    .OrderByDescending(x => x.WeekNumber)
                    .ThenByDescending(x => x.SequenceNumber)
                    .FirstOrDefault();


                if (lastLesson != null && lastLesson.TestType == null)
                {
                    response.Success = false;
                    response.Message = $"The last lesson of learning outcome '{learningOutcome.Name}' is not a test.";
                }
            }

            return Task.FromResult(response);
        }

    }
}

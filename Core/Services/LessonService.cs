using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Domain.Models;
using Core.Interfaces.Repositories;

namespace Core.Services;

public class LessonService : ILessonService
{
    private readonly IRepository<Lesson> lessonRepository;
    private readonly IRepository<Planning> planningRepository;
    private readonly IRepository<LearningOutcome> learningOutcomeRepository;
    private readonly IMapper mapper;

    public LessonService(IRepository<Lesson> lessonRepository, IRepository<Planning> planningRepository, IRepository<LearningOutcome> learningOutcomeRepository, IMapper mapper)
    {
        this.lessonRepository = lessonRepository;
        this.planningRepository = planningRepository;
        this.learningOutcomeRepository = learningOutcomeRepository;
        this.mapper = mapper;
    }

    public async Task<Response<LessonDTO>> CreateLesson(CreateLessonDTO createLessonDTO)
    {
        try
        {
            var planning = await planningRepository.Get(createLessonDTO.PlanningId);

            if (planning == null)
            {
                return Response<LessonDTO>.Fail("Planning not found");
            }

            var newLesson = new Lesson
            {
                WeekNumber = createLessonDTO.WeekNumber,
                Name = createLessonDTO.Name,
                Description = createLessonDTO.Description,
                SequenceNumber = createLessonDTO.SequenceNumber,
                PlanningId = createLessonDTO.PlanningId,
                TestType = createLessonDTO.TestType.HasValue ? (Domain.Enums.TestType)createLessonDTO.TestType : null,
                Weight = createLessonDTO.Weight
            };

            await lessonRepository.CreateAndCommit(newLesson);

            return Response<LessonDTO>.Ok(mapper.Map<LessonDTO>(newLesson));
        }
        catch (Exception ex)
        {
            return Response<LessonDTO>.Fail("Error creating lesson: " + ex.Message);
        }
    }

    public async Task<Response<LessonDTO>> GetLessonById(int lessonId)
    {
        try
        {
            var lesson = await lessonRepository.Get(lessonId);

            if (lesson == null)
            {
                return Response<LessonDTO>.Fail("Lesson not found");
            }

            return Response<LessonDTO>.Ok(mapper.Map<LessonDTO>(lesson));
        }
        catch (Exception ex)
        {
            return Response<LessonDTO>.Fail("Error retrieving lesson: " + ex.Message);
        }
    }

    public async Task<Response<IList<LessonDTO>>> GetAllLessonsByPlanningId(int planningId)
    {
        try
        {
            var lessons = await lessonRepository.GetAll(x => x.PlanningId == planningId);

            return Response<IList<LessonDTO>>.Ok(mapper.Map<IList<LessonDTO>>(lessons));
        }
        catch (Exception ex)
        {
            return Response<IList<LessonDTO>>.Fail("Error retrieving lessons: " + ex.Message);
        }
    }

    public async Task<Response<LessonDTO>> UpdateLesson(UpdateLessonDTO updateLessonDTO)
    {
        try
        {
            var lesson = await lessonRepository.Get(updateLessonDTO.Id);

            if (lesson == null)
            {
                return Response<LessonDTO>.Fail("Lesson not found");
            }

            lesson.WeekNumber = updateLessonDTO.WeekNumber;
            lesson.Name = updateLessonDTO.Name;
            lesson.Description = updateLessonDTO.Description;
            lesson.SequenceNumber = updateLessonDTO.SequenceNumber;
            lesson.TestType = updateLessonDTO.TestType.HasValue ? (Domain.Enums.TestType)updateLessonDTO.TestType : null;
            lesson.Weight = updateLessonDTO.Weight;

            await lessonRepository.UpdateAndCommit(lesson);

            return Response<LessonDTO>.Ok(mapper.Map<LessonDTO>(lesson));
        }
        catch (Exception ex)
        {
            return Response<LessonDTO>.Fail("Error updating lesson: " + ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteLesson(int lessonId)
    {
        try
        {
            var lesson = await lessonRepository.Get(lessonId);

            if (lesson == null)
            {
                return Response<bool>.Fail("Lesson not found");
            }

            await lessonRepository.DeleteAndCommit(lessonId);

            return Response<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Response<bool>.Fail("Error deleting lesson: " + ex.Message);
        }
    }

    public async Task<Response<bool>> AddLearningOutcomeToLesson(int lessonId, int learningOutcomeId)
    {
        try
        {
            var lesson = await lessonRepository.Get(lessonId);
            if (lesson == null)
                return Response<bool>.Fail("Lesson not found");

            var learningOutcome = await learningOutcomeRepository.Get(learningOutcomeId);
            if (learningOutcome == null)
                return Response<bool>.Fail("Learning outcome not found");

            // ensure lists are initialized
            lesson.LearningOutcomes ??= new List<LearningOutcome>();
            learningOutcome.Lessons ??= new List<Lesson>();

            // prevent duplicates
            if (lesson.LearningOutcomes.Any(lo => lo.Id == learningOutcomeId))
                return Response<bool>.Fail("Learning outcome already attached to lesson");

            lesson.LearningOutcomes.Add(learningOutcome);
            learningOutcome.Lessons.Add(lesson);

            await lessonRepository.UpdateAndCommit(lesson);

            return Response<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Response<bool>.Fail("Error attaching learning outcome to lesson: " + ex.Message);
        }
    }

    public async Task<Response<bool>> RemoveLearningOutcomeFromLesson(int lessonId, int learningOutcomeId)
    {
        try
        {
            var lesson = await lessonRepository.Get(lessonId);
            if (lesson == null)
                return Response<bool>.Fail("Lesson not found");

            var learningOutcome = await learningOutcomeRepository.Get(learningOutcomeId);
            if (learningOutcome == null)
                return Response<bool>.Fail("Learning outcome not found");

            lesson.LearningOutcomes ??= new List<LearningOutcome>();
            learningOutcome.Lessons ??= new List<Lesson>();

            var toRemoveFromLesson = lesson.LearningOutcomes.FirstOrDefault(lo => lo.Id == learningOutcomeId);
            if (toRemoveFromLesson == null)
                return Response<bool>.Fail("Learning outcome not attached to lesson");

            lesson.LearningOutcomes.Remove(toRemoveFromLesson);

            var toRemoveFromLO = learningOutcome.Lessons.FirstOrDefault(l => l.Id == lessonId);
            if (toRemoveFromLO != null)
                learningOutcome.Lessons.Remove(toRemoveFromLO);

            await lessonRepository.UpdateAndCommit(lesson);

            return Response<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Response<bool>.Fail("Error detaching learning outcome from lesson: " + ex.Message);
        }
    }
}

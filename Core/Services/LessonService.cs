using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Core.Interfaces.Repositories;
using Domain.Models;

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

    public async Task<Response<LessonDto>> CreateLesson(CreateLessonDto createLessonDTO)
    {
        try
        {
            var planning = await planningRepository.Get(createLessonDTO.PlanningId);

            if (planning == null)
            {
                return Response<LessonDto>.Fail("Planning not found");
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

            return Response<LessonDto>.Ok(mapper.Map<LessonDto>(newLesson));
        }
        catch (Exception ex)
        {
            return Response<LessonDto>.Fail("Error creating lesson: " + ex.Message);
        }
    }

    public async Task<Response<LessonDto>> GetLessonById(int lessonId)
    {
        try
        {
            var lesson = await lessonRepository.Get(lessonId);

            if (lesson == null)
            {
                return Response<LessonDto>.Fail("Lesson not found");
            }

            return Response<LessonDto>.Ok(mapper.Map<LessonDto>(lesson));
        }
        catch (Exception ex)
        {
            return Response<LessonDto>.Fail("Error retrieving lesson: " + ex.Message);
        }
    }

    public async Task<Response<IList<LessonDto>>> GetAllLessonsByPlanningId(int planningId)
    {
        try
        {
            var lessons = await lessonRepository.GetAll(x => x.PlanningId == planningId);

            return Response<IList<LessonDto>>.Ok(mapper.Map<IList<LessonDto>>(lessons));
        }
        catch (Exception ex)
        {
            return Response<IList<LessonDto>>.Fail("Error retrieving lessons: " + ex.Message);
        }
    }

    public async Task<Response<LessonDto>> UpdateLesson(UpdateLessonDto updateLessonDTO)
    {
        try
        {
            var lesson = await lessonRepository.Get(updateLessonDTO.Id);

            if (lesson == null)
            {
                return Response<LessonDto>.Fail("Lesson not found");
            }

            lesson.WeekNumber = updateLessonDTO.WeekNumber;
            lesson.Name = updateLessonDTO.Name;
            lesson.Description = updateLessonDTO.Description;
            lesson.SequenceNumber = updateLessonDTO.SequenceNumber;
            lesson.TestType = updateLessonDTO.TestType.HasValue ? (Domain.Enums.TestType)updateLessonDTO.TestType : null;
            lesson.Weight = updateLessonDTO.Weight;

            await lessonRepository.UpdateAndCommit(lesson);

            return Response<LessonDto>.Ok(mapper.Map<LessonDto>(lesson));
        }
        catch (Exception ex)
        {
            return Response<LessonDto>.Fail("Error updating lesson: " + ex.Message);
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


    public async Task<Response<bool>> AddLearningOutcomesToLesson(int lessonId, IList<int> learningOutcomeIds)
    {
        try
        {
            var lessonQuery = lessonRepository.Include(l => l.LearningOutcomes);
            var lesson = await lessonRepository.FirstOrDefaultAsync(lessonQuery.Where(l => l.Id == lessonId));
            if (lesson == null)
                return Response<bool>.Fail("Lesson not found");

            lesson.LearningOutcomes ??= new List<LearningOutcome>();

            // Load all requested learning outcomes in one call
            var loQuery = learningOutcomeRepository.Include(lo => lo.Lessons).Where(lo => learningOutcomeIds.Contains(lo.Id));
            var learningOutcomes = await learningOutcomeRepository.ToListAsync(loQuery);
            var loById = learningOutcomes.ToDictionary(l => l.Id);

            var toUpdateLOs = new List<LearningOutcome>();

            foreach (var loId in learningOutcomeIds.Distinct())
            {
                if (!loById.TryGetValue(loId, out var learningOutcome))
                    return Response<bool>.Fail($"Learning outcome not found: {loId}");

                learningOutcome.Lessons ??= new List<Lesson>();

                if (!lesson.LearningOutcomes.Any(lo => lo.Id == loId))
                {
                    lesson.LearningOutcomes.Add(learningOutcome);
                    if (!learningOutcome.Lessons.Any(l => l.Id == lessonId))
                    {
                        learningOutcome.Lessons.Add(lesson);
                        toUpdateLOs.Add(learningOutcome);
                    }
                }
            }

            if (toUpdateLOs.Count == 0)
            {
                return Response<bool>.NotFound($"No records found to update");
            }

            // Persist changed learning outcomes first (reduces risk of join not being created)
            foreach (var lo in toUpdateLOs)
            {
                await learningOutcomeRepository.UpdateAndCommit(lo);
            }

            await lessonRepository.UpdateAndCommit(lesson);

            return Response<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Response<bool>.Fail("Error attaching learning outcomes to lesson: " + ex.Message);
        }
    }

    public async Task<Response<bool>> RemoveLearningOutcomesFromLesson(int lessonId, IList<int> learningOutcomeIds)
    {
        try
        {
            var lessonQuery = lessonRepository.Include(l => l.LearningOutcomes);
            var lesson = await lessonRepository.FirstOrDefaultAsync(lessonQuery.Where(l => l.Id == lessonId));
            if (lesson == null)
                return Response<bool>.Fail("Lesson not found");

            lesson.LearningOutcomes ??= new List<LearningOutcome>();

            // Load all requested learning outcomes in one call
            var loQuery = learningOutcomeRepository.Include(lo => lo.Lessons).Where(lo => learningOutcomeIds.Contains(lo.Id));
            var learningOutcomes = await learningOutcomeRepository.ToListAsync(loQuery);
            var loById = learningOutcomes.ToDictionary(l => l.Id);

            var toUpdateLOs = new List<LearningOutcome>();

            foreach (var loId in learningOutcomeIds.Distinct())
            {
                if (!loById.TryGetValue(loId, out var learningOutcome))
                    return Response<bool>.Fail($"Learning outcome not found: {loId}");

                learningOutcome.Lessons ??= new List<Lesson>();

                var toRemoveFromLesson = lesson.LearningOutcomes.FirstOrDefault(lo => lo.Id == loId);
                if (toRemoveFromLesson != null)
                    lesson.LearningOutcomes.Remove(toRemoveFromLesson);

                var toRemoveFromLO = learningOutcome.Lessons.FirstOrDefault(l => l.Id == lessonId);
                if (toRemoveFromLO != null)
                {
                    learningOutcome.Lessons.Remove(toRemoveFromLO);
                    toUpdateLOs.Add(learningOutcome);
                }
            }
            if (toUpdateLOs.Count == 0)
            {
                return Response<bool>.NotFound($"No records found to update");
            }
            // Persist changed learning outcomes
            foreach (var lo in toUpdateLOs)
            {
                await learningOutcomeRepository.UpdateAndCommit(lo);
            }

            await lessonRepository.UpdateAndCommit(lesson);

            return Response<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Response<bool>.Fail("Error detaching learning outcomes from lesson: " + ex.Message);
        }
    }
}

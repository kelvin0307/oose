using AutoMapper;
using Core.Common;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Core.Extensions.ModelExtensions;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Services.Abstractions;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;
public class PlanningService : Generatable<Planning>, IPlanningService
{
    private readonly IRepository<Planning> planningRepository;
    private readonly IMapper mapper;

    public PlanningService(
        IRepository<Planning> planningRepository,
        IDocumentFactory documentFactory,
        IMapper mapper)
        : base(documentFactory)
    {
        this.planningRepository = planningRepository;
        this.mapper = mapper;
    }

    public Response<PlanningDTO> GetPlanningByCourseId(int courseId)
    {
        try
        {
            var planning = planningRepository
                .Include(x => x.Lessons)
                .FirstOrDefault(x => x.CourseId == courseId)
                ?.ToDto(mapper);

            return planning != null
            ? Response<PlanningDTO>.Ok(planning)
            : Response<PlanningDTO>.NotFound("planning not found");
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<PlanningDTO>.Fail("Invalid operation while getting course", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<PlanningDTO>.Fail("An unexpected error occurred while fetching the course");
        }
    }

    //public async Task<Response<PlanningDTO>> CreatePlanning(CreatePlanningDTO planningDTO)
    //{
    //    try
    //    {
    //        var domainLessons = planningDTO.Lessons
    //        .Select(mapper.Map<Lesson>)
    //        .ToList();

    //        var planning = await planningRepository.CreateAndCommit(new Planning() { CourseId = planningDTO.CourseId, Lessons = domainLessons });

    //        return Response<PlanningDTO>.Ok(planning.ToDto(mapper));
    //    }
    //    catch (Exception)
    //    {
    //        //TODO: Log exception
    //        return Response<PlanningDTO>.Fail("An unexpected error occurred while fetching the course");
    //    }
    //}



    #region Generatable Members

    public override Task<Response<DocumentDTO>> GenerateDocument(int courseId, DocumentTypes documentType)
    {
        try
        {
            var planning = planningRepository
                .Include(x => x.Lessons)
                .FirstOrDefault(x => x.CourseId == courseId);

            if (planning == null)
            {
                return Task.FromResult(Response<DocumentDTO>.Fail("Error generating planning document"));
            }

            var documentData = MapToDocumentDataDTO(planning);

            return Task.FromResult(Response<DocumentDTO>.Ok(CreateDocument(documentData, documentType)));
        }
        catch (Exception ex)
        {
            //TODO: Add logging here.
            return Task.FromResult(Response<DocumentDTO>.Fail("Error generating planning document" + ex.Message));
        }
    }

    protected override DocumentDataDTO MapToDocumentDataDTO(Planning planning)
    {
        var paragraphs = planning.Lessons?.ToDictionary(x => x.SequenceNumber + ". " + x.Name, x => "Week " + x.WeekNumber.ToString());

        return new DocumentDataDTO()
        {
            Title = "Planning",
            Paragraphs = paragraphs ?? [],
        };
    }
    #endregion
}

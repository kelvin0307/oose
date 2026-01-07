using AutoMapper;
using Core.Common;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Core.Extensions.ModelExtensions;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;
public class PlanningService : Generatable<PlanningDTO>, IPlanningService
{
    private readonly IRepository<Planning> planningRepository;
    private readonly IRepository<Lesson> lessonRepository;
    private readonly IDocumentFactory documentFactory;
    private readonly IMapper mapper;

    public PlanningService(
        IRepository<Planning> planningRepository,
        IRepository<Lesson> lessonRepository,
        IDocumentFactory documentFactory,
        IMapper mapper)
        : base(documentFactory)
    {
        this.planningRepository = planningRepository;
        this.lessonRepository = lessonRepository;
        this.documentFactory = documentFactory;
        this.mapper = mapper;
    }

    public Response<PlanningDTO> GetPlanningByCourseId(int courseId)
    {
        try
        {
            var planning = GetByCourseId(courseId);

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

    #region Generatable Members

    public override DocumentDTO GenerateDocument(int courseId, DocumentTypes documentType)
    {
        try
        {
            var planning = GetByCourseId(courseId);

            if (planning == null)
            {
                throw new Exception("Error generating planning document");
            }

            var documentData = MapToDocumentDataDTO(planning);

            return documentFactory.GenerateDocument(documentData, documentType);
        }
        catch (Exception ex)
        {
            //TODO: Add logging here.
            throw new Exception("Error generating planning document", ex);
        }
    }

    protected override DocumentDataDTO MapToDocumentDataDTO(PlanningDTO planning)
    {
        var paragraphs = planning.Lessons?.ToDictionary(x => x.SequenceNumber + ". " + x.Name, x => "Week " + x.WeekNumber.ToString());

        return new DocumentDataDTO()
        {
            Title = "Planning",
            Paragraphs = paragraphs ?? [],
        };
    }

    private PlanningDTO? GetByCourseId(int courseId)
    {
        var planning = planningRepository.GetByCourseId(courseId).FirstOrDefault();

        if (planning == null)
        {
            return null;
        }

        var lessons = lessonRepository.GetByPlanningId(planning.Id).ToArray();

        return new PlanningDTO
        {
            Id = planning.Id,
            Lessons = lessons.Select(x => x.ToDto(mapper)).ToArray()
        };
    }
    #endregion
}

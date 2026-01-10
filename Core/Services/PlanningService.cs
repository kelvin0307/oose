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
public class PlanningService : Generatable<PlanningDTO>, IPlanningService
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

    public override Task<Response<DocumentDTO>> GenerateDocument(int courseId, DocumentTypes documentType)
    {
        try
        {
            var planning = GetByCourseId(courseId);

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
        var planning = planningRepository.Include(x => x.Lessons).GetByCourseId(courseId).FirstOrDefault();

        if (planning == null)
        {
            return null;
        }

        return new PlanningDTO
        {
            Id = planning.Id,
            Lessons = planning.Lessons?.Select(x => x.ToDto(mapper)).ToArray()
        };
    }
    #endregion
}

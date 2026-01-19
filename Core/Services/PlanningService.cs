using AutoMapper;
using Core.Common;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Core.Enums;
using Core.Extensions.Mapper;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services.Abstractions;
using Domain.Models;

namespace Core.Services;
public class PlanningService : Generatable<Planning, int>, IPlanningService
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

    public Response<PlanningDto> GetPlanningByCourseId(int courseId)
    {
        try
        {
            var planning = planningRepository
                .Include(x => x.Lessons)
                .FirstOrDefault(x => x.CourseId == courseId)
                ?.ToDto(mapper);

            return planning != null
            ? Response<PlanningDto>.Ok(planning)
            : Response<PlanningDto>.NotFound("planning not found");
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<PlanningDto>.Fail("Invalid operation while getting course", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<PlanningDto>.Fail("An unexpected error occurred while fetching the course");
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

    public override Task<Response<DocumentDto>> GenerateDocument(int courseId, DocumentType documentType)
    {
        try
        {
            var planning = planningRepository
                .Include(x => x.Lessons)
                .FirstOrDefault(x => x.CourseId == courseId);

            if (planning == null)
            {
                return Task.FromResult(Response<DocumentDto>.Fail("Error generating planning document"));
            }

            var documentData = MapToDocumentDataDTO(planning);

            return Task.FromResult(Response<DocumentDto>.Ok(CreateDocument(documentData, documentType)));
        }
        catch (Exception ex)
        {
            //TODO: Add logging here.
            return Task.FromResult(Response<DocumentDto>.Fail("Error generating planning document" + ex.Message));
        }
    }

    protected override DocumentDataDto MapToDocumentDataDTO(Planning planning)
    {
        var paragraphs = planning.Lessons?.ToDictionary(x => x.SequenceNumber + ". " + x.Name, x => "Week " + x.WeekNumber.ToString());

        return new DocumentDataDto()
        {
            Title = "Planning",
            Paragraphs = paragraphs ?? [],
        };
    }
    #endregion
}

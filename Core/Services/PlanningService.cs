using Core.DocumentGenerator.Factories;
using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Domain.Models;

namespace Core.Services;
public class PlanningService : Generatable<Planning>, IPlanningService
{
    private readonly IRepository<Planning> planningRepository;

    public PlanningService(IRepository<Planning> planningRepository, IDocumentFactory documentFactory) : base(documentFactory)
    {
        this.planningRepository = planningRepository;

    }

    public Task<PlanningDTO?> GetPlanningByCourseId(int courseId)
    {
        //return planningRepository.Get(x => x.CourseId == courseId);
        return null;
    }

    #region Generatable Members

    public override async Task<byte[]> GenerateDocument(int id)
    {
        try
        {
            // first get the correct planning by courseId
            //var planning = await planningRepository.Get(x => x.CourseId == id) ?? throw new Exception("Planning to generate not found");
            //var documentData = MapToDocumentDataDTO(planning);
            return [];

        }
        catch (Exception ex)
        {
            //TODO: Add logging here.
            throw new Exception("Error generating planning document", ex);
        }
    }

    protected override DocumentDataDTO MapToDocumentDataDTO(Planning planning)
    {
        var paragraphs = planning.Lessons?.ToDictionary(x => x.SequenceNumber + x.Name, x => x.WeekNumber.ToString());

        return new DocumentDataDTO()
        {
            Paragraphs = paragraphs,
        };
    }

    #endregion
}

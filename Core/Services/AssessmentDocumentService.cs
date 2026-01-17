using Core.Common;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Core.Interfaces.Services;
using Core.Services.Abstractions;
using Data.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;

public class AssessmentDocumentService : Generatable<IList<Rubric>, int>, IAssessmentDocumentService
{
    private readonly IRubricRepository rubricRepository;
    private readonly IRepository<Lesson> lessonRepository;

    public AssessmentDocumentService(
        IRepository<Lesson> lessonRepository,
        IDocumentFactory documentFactory,
        IRubricRepository rubricRepository)
        : base(documentFactory)
    {
        this.lessonRepository = lessonRepository;
        this.rubricRepository = rubricRepository;
    }

    public override async Task<Response<DocumentDTO>> GenerateDocument(int testId, DocumentTypes documentType)
    {
        try
        {
            var test = lessonRepository
                .Include(x => x.LearningOutcomes)
                .FirstOrDefault(x => x.Id == testId);

            if (test == null)
            {
                return Response<DocumentDTO>.NotFound("Error generating assessment document: Test not found");
            }
            if (test.TestType == null)
            {
                return Response<DocumentDTO>.NotFound("Error generating assessment document: Id is not a Test");
            }


            var rubrics = await rubricRepository.GetAggregatesByLearningOutcomeIds(test.LearningOutcomes.Select(x => x.Id).ToArray());

            var doc = MapToDocumentDataDTO(rubrics);

            return Response<DocumentDTO>.Ok(CreateDocument(doc, documentType));

        }
        catch (Exception ex)
        {
            return Response<DocumentDTO>.Fail("Error generating assessment document" + ex.Message);
        }
    }

    protected override DocumentDataDTO MapToDocumentDataDTO(IList<Rubric> rubrics)
    {
        var paragraphs = new Dictionary<string, string>();

        foreach (var rubric in rubrics)
        {
            // rubric
            paragraphs.Add($"{rubric.Id} - Rubric:", rubric.Name);


            foreach (var dimension in rubric.AssessmentDimensions)
            {
                var scoreString = string.Join(", ", dimension.AssessmentDimensionScores.Select(s => s.Score.ToString()));
                paragraphs.Add($"{dimension}:", scoreString);
            }
        }

        return new DocumentDataDTO()
        {
            Title = "AssessmentDocument",
            Paragraphs = paragraphs ?? [],
        };
    }
}

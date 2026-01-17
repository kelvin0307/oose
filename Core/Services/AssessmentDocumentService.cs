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

            var loIds = test.LearningOutcomes.Select(x => x.Id).ToArray();
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

        var rubricIndex = 1;

        foreach (var rubric in rubrics)
        {
            paragraphs.Add(
                $"{rubricIndex}. Rubric",
                rubric.Name
            );

            paragraphs.Add($"\t\t\t\t\t\t{MakeUniqueKey("Assessment dimensions", rubricIndex)}:", "");

            var dimensionIndex = 1;

            foreach (var dimension in rubric.AssessmentDimensions)
            {
                paragraphs.Add(
                    $"\t\t\t\t\t\t\t\t\t\t\t{dimensionIndex}. {dimension.Name}",
                    $"\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t{dimension.NameCriterium}"
                );

                var scoresText = string.Join(
                    "\r\n",
                    dimension.AssessmentDimensionScores.Select(s =>
                        $"\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t Score {s.Score}: {s.Description}"
                    )
                );

                paragraphs.Add(
                    $"\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t{MakeUniqueKey("Scores", rubricIndex * dimensionIndex * new Random().Next(0, 10))}: ",
                    MakeUniqueKey(scoresText, rubricIndex * dimensionIndex * new Random().Next(0, 10))
                );

                dimensionIndex++;
            }

            rubricIndex++;
        }
        
        return new DocumentDataDTO()
        {
            Title = "AssessmentDocument",
            Paragraphs = paragraphs ?? [],
        };
    }
    
    private static string MakeUniqueKey(string baseText, int uniqueIndex)
    {
        return baseText + new string('\u200B', uniqueIndex);
    }}

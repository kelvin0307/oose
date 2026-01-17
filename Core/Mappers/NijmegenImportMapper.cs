using Core.DTOs.Imports.Nijmegen;
using Domain.Enums;
using Domain.Models;

namespace Core.Import.Nijmegen;

public static class NijmegenImportMapper
{
    
    public static Course Map(NijmegenImportDataDto importData)
         {
        if (importData?.Course == null)
            throw new ArgumentNullException(nameof(importData), "Import data or course cannot be null");

        var course = new Course
        {
            Name = importData.Course.Naam ?? string.Empty,
            Description = importData.Course.Beschrijving ?? string.Empty,
            Status = (CourseStatus)importData.Course.Status,
            LearningOutcomes = new List<LearningOutcome>(),
            Planning = null
        };

        // Map Learning Outcomes
        var learningOutcomesMap = new Dictionary<int, LearningOutcome>();
        foreach (var nijmegenLo in importData.LearningOutcomes)
        {
            var learningOutcome = new LearningOutcome
            {
                Name = nijmegenLo.Naam,
                Description = nijmegenLo.Beschrijving,
                EndQualification = nijmegenLo.Eindkwalificatie,
                Course = course,
                Lessons = new List<Lesson>(),
                AssessmentDimensions = new List<AssessmentDimension>()
            };

            course.LearningOutcomes.Add(learningOutcome);
            learningOutcomesMap[nijmegenLo.Id] = learningOutcome;
        }

        // Map Rubrics to Learning Outcomes
        foreach (var nijmegenRubric in importData.Rubrics)
        {
            if (learningOutcomesMap.TryGetValue(nijmegenRubric.LearningOutcomeId, out var learningOutcome))
            {
                var rubric = new Rubric
                {
                    Name = nijmegenRubric.Name,
                    LearningOutcome = learningOutcome,
                    AssessmentDimensions = new List<AssessmentDimension>()
                };

                // Map Assessment Dimensions
                foreach (var nijmegenDimension in nijmegenRubric.AssessmentDimensions)
                {
                    var assessmentDimension = new AssessmentDimension
                    {
                        Name = nijmegenDimension.Name,
                        NameCriterium = nijmegenDimension.NameCriterium,
                        Wage = nijmegenDimension.Wage,
                        MinimumScore = nijmegenDimension.MinimumScore,
                        Rubric = rubric,
                        AssessmentDimensionScores = new List<AssessmentDimensionScore>()
                    };

                    // Map Assessment Dimension Scores
                    foreach (var nijmegenScore in nijmegenDimension.AssessmentDimensionScores)
                    {
                        var score = new AssessmentDimensionScore
                        {
                            Score = nijmegenScore.Score,
                            Description = nijmegenScore.Description,
                            AssessmentDimension = assessmentDimension
                        };

                        assessmentDimension.AssessmentDimensionScores.Add(score);
                    }

                    rubric.AssessmentDimensions.Add(assessmentDimension);
                    learningOutcome.AssessmentDimensions.Add(assessmentDimension);
                }
            }
        }

        // Map Planning (take first planning if multiple exist)
        if (importData.Planning.Any())
        {
            var nijmegenPlanning = importData.Planning.First();
            
            var planning = new Planning
            {
                Course = course,
                Lessons = new List<Lesson>()
            };

            // Map Lessons
            if (nijmegenPlanning.Lessons != null)
            {
                foreach (var nijmegenLesson in nijmegenPlanning.Lessons)
                {
                    var lesson = new Lesson
                    {
                        WeekNumber = nijmegenLesson.Weeknummer,
                        Name = nijmegenLesson.Naam,
                        SequenceNumber = nijmegenLesson.SequenceNumber,
                        TestType = nijmegenLesson.TestVariant,
                        Planning = planning,
                        Materials = new List<Material>(),
                        LearningOutcomes = new List<LearningOutcome>(),
                        Grades = new List<Grade>()
                    };

                    planning.Lessons.Add(lesson);
                }
            }

            course.Planning = planning;
        }

        return course;
    }
}
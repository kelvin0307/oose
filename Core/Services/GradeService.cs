using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;

public class GradeService(
    IRepository<Grade> gradeRepository,
    IRepository<Student> studentRepository,
    IRepository<Lesson> lessonRepository,
    IRepository<CourseExecution> courseExecutionRepository,
    IMapper mapper) : IGradeService
{
    // Use enum for letter grades
    private static bool TryParseLetterGrade(string value, out int numeric)
    {
        numeric = 0;
        if (string.IsNullOrWhiteSpace(value)) return false;

        if (int.TryParse(value, out var parsed))
        {
            if (parsed >= 1 && parsed <= 10)
            {
                numeric = parsed;
                return true;
            }

            return false;
        }

        // Try parse enum (case-insensitive)
        if (Enum.TryParse<LetterGrade>(value.Trim(), true, out var letter))
        {
            numeric = (int)letter;
            return true;
        }

        return false;
    }

    public async Task<Response<GradeDto>> CreateGrade(CreateGradeDto createGradeDTO)
    {
        try
        {
            if (!TryParseLetterGrade(createGradeDTO.Grade, out var numericGrade))
                return Response<GradeDto>.Fail("Invalid or missing grade");

            var lesson = await lessonRepository.Get(createGradeDTO.LessonId);
            if (lesson == null)
                return Response<GradeDto>.NotFound("Lesson not found");

            if (lesson.TestType == null)
                return Response<GradeDto>.Fail("Only exam lessons (lessons with a TestType) can have grades");

            var grade = new Grade
            {
                GradeValue = numericGrade,
                Feedback = createGradeDTO.Feedback ?? string.Empty,
                StudentId = createGradeDTO.StudentId,
                LessonId = createGradeDTO.LessonId,
                CourseExcecutionId = createGradeDTO.CourseExecutionId
            };

            var created = await gradeRepository.CreateAndCommit(grade);

            var student = await studentRepository.Get(createGradeDTO.StudentId);
            grade.Student = student;
            return Response<GradeDto>.Ok(mapper.Map<GradeDto>(grade));
        }
        catch (Exception ex)
        {
            return Response<GradeDto>.Fail($"Error creating grade: {ex.Message}");
        }
    }

    public async Task<Response<GradeDto>> UpdateGrade(UpdateGradeDto updateGradeDTO)
    {
        try
        {
            var existing = await gradeRepository.Get(updateGradeDTO.Id);
            if (existing == null)
                return Response<GradeDto>.NotFound("Grade not found");

            if (!string.IsNullOrEmpty(updateGradeDTO.Grade))
            {
                if (!TryParseLetterGrade(updateGradeDTO.Grade, out var numericGrade))
                    return Response<GradeDto>.Fail("Invalid or missing grade");

                existing.GradeValue = numericGrade;
            }

            existing.Feedback = updateGradeDTO.Feedback ?? string.Empty;

            var updated = await gradeRepository.UpdateAndCommit(existing);

            var gradeWithStudent = gradeRepository.Include(g => g.Student).Where(g => g.Id == updated.Id).FirstOrDefault();

            return Response<GradeDto>.Ok(mapper.Map<GradeDto>(gradeWithStudent));
        }
        catch (Exception ex)
        {
            return Response<GradeDto>.Fail($"Error updating grade: {ex.Message}");
        }
    }

    public async Task<Response<List<GradeDto>>> GetLatestGradesByClassAndExecution(int classId, int courseExecutionId)
    {
        try
        {
            // Eagerly load student and filter by student's ClassId to avoid separate student query
            var query = gradeRepository
                .Include(g => g.Student)
                .Where(g => g.CourseExcecutionId == courseExecutionId && g.Student != null && g.Student.ClassId == classId);

            var allGrades = await gradeRepository.ToListAsync(query);

            var latest = allGrades
                .GroupBy(g => new { g.StudentId, g.LessonId })
                .Select(g => g.OrderByDescending(x => x.Id).First())
                .ToList();

            var dtos = latest.Select(mapper.Map<GradeDto>)
                .OrderBy(d => d.StudentLastName)
                .ThenBy(d => d.StudentFirstName)
                .ToList();

            return Response<List<GradeDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Response<List<GradeDto>>.Fail($"Error retrieving grades: {ex.Message}");
        }
    }


}

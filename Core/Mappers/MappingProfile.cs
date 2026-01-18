using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Material, MaterialDto>().ReverseMap();
        CreateMap<Course, CourseDto>().ReverseMap();
        CreateMap<CreateCourseDto, Course>();
        CreateMap<Lesson, LessonDto>().ReverseMap();
        CreateMap<CreateLessonDto, Lesson>();
        CreateMap<UpdateLessonDto, Lesson>();
        
        CreateMap<Rubric, RubricDto>().ReverseMap();
        CreateMap<AssessmentDimension, AssessmentDimensionDto>().ReverseMap();
        CreateMap<AssessmentDimensionScore, AssessmentDimensionScoreDto>().ReverseMap();
        
        CreateMap<CreateRubricDto, Rubric>().ReverseMap();
        CreateMap<CreateAssessmentDimensionDto, AssessmentDimension>().ReverseMap();
        CreateMap<CreateAssessmentDimensionScoreDto, AssessmentDimensionScore>().ReverseMap();
        
        CreateMap<UpdateRubricDto, Rubric>().ReverseMap();
        CreateMap<UpdateAssessmentDimensionDto, AssessmentDimension>().ReverseMap();
        CreateMap<UpdateAssessmentDimensionScoreDto, AssessmentDimensionScore>().ReverseMap();

        CreateMap<Student, StudentDto>().ReverseMap();
        CreateMap<Class, ClassDto>().ReverseMap();
        CreateMap<GradeDto, Grade>();
        CreateMap<CreateGradeDto, Grade>();
        CreateMap<Grade, GradeDto>()
            .ForMember(dest => dest.StudentFirstName,
                opt => opt.MapFrom(src => src.Student != null ? src.Student.FirstName : string.Empty))
            .ForMember(dest => dest.StudentLastName,
                opt => opt.MapFrom(src => src.Student != null ? src.Student.LastName : string.Empty));
        
        CreateMap<CourseExecution,CourseExecutionDto>().ReverseMap();
    }
}
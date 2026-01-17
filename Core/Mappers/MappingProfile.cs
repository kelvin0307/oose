using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Material, MaterialDTO>().ReverseMap();
        CreateMap<Course, CourseDTO>().ReverseMap();
        CreateMap<CreateCourseDto, Course>();
        CreateMap<Lesson, LessonDTO>();
        
        CreateMap<Rubric, RubricDto>().ReverseMap();
        CreateMap<AssessmentDimension, AssessmentDimensionDto>().ReverseMap();
        CreateMap<AssessmentDimensionScore, AssessmentDimensionScoreDto>().ReverseMap();
        
        CreateMap<CreateRubricDto, Rubric>().ReverseMap();
        CreateMap<CreateAssessmentDimensionDto, AssessmentDimension>().ReverseMap();
        CreateMap<CreateAssessmentDimensionScoreDto, AssessmentDimensionScore>().ReverseMap();
        
        CreateMap<UpdateRubricDto, Rubric>().ReverseMap();
        CreateMap<UpdateAssessmentDimensionDto, AssessmentDimension>().ReverseMap();
        CreateMap<UpdateAssessmentDimensionScoreDto, AssessmentDimensionScore>().ReverseMap();

        CreateMap<Student, StudentDTO>().ReverseMap();
        CreateMap<Class, ClassDTO>().ReverseMap();
        CreateMap<GradeDTO, Grade>();
        CreateMap<CreateGradeDTO, Grade>();
        CreateMap<Grade, GradeDTO>()
            .ForMember(dest => dest.StudentFirstName,
                opt => opt.MapFrom(src => src.Student != null ? src.Student.FirstName : string.Empty))
            .ForMember(dest => dest.StudentLastName,
                opt => opt.MapFrom(src => src.Student != null ? src.Student.LastName : string.Empty));
    }
}
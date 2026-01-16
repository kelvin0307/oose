using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseDto>().ReverseMap();
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

    }
}
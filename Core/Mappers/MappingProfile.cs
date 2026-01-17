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
        CreateMap<Student, StudentDTO>().ReverseMap();
        CreateMap<Class, ClassDTO>().ReverseMap();
    }
}

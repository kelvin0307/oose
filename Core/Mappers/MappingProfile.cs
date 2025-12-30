using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>().ReverseMap();
        }
    }
}

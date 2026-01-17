using AutoMapper;
using Core.Common;
using Core.DTOs;
using Data.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class ClassService(IRepository<Class> classRepository, IRepository<Student> studentRepository, IMapper mapper) : IClassService
{
    public async Task<Response<ClassDTO>> GetClassById(int classId)
    {
        try
        {
            var query = classRepository.Include(c => c.Students).Where(c => c.Id == classId);
            var classEntity = await classRepository.FirstOrDefaultAsync(query);

            if (classEntity == null)
                return Response<ClassDTO>.NotFound("Class not found");

            var classDto = mapper.Map<ClassDTO>(classEntity);
            return Response<ClassDTO>.Ok(classDto);
        }
        catch (InvalidOperationException)
        {
            return Response<ClassDTO>.Fail("Invalid operation while fetching class", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<ClassDTO>.Fail("An unexpected error occurred while fetching the class");
        }
    }

    public async Task<Response<List<ClassDTO>>> GetAllClasses()
    {
        try
        {
            var classes = await classRepository.GetAll();
            var classDtos = classes.Select(c => mapper.Map<ClassDTO>(c)).ToList();
            return Response<List<ClassDTO>>.Ok(classDtos);
        }
        catch (InvalidOperationException)
        {
            return Response<List<ClassDTO>>.Fail("Invalid operation while fetching classes", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<List<ClassDTO>>.Fail("An unexpected error occurred while fetching classes");
        }
    }

    public async Task<Response<List<StudentDTO>>> GetStudentsByClassId(int classId)
    {
        try
        {
            var classEntity = await classRepository.Get(classId);
            if (classEntity == null)
                return Response<List<StudentDTO>>.NotFound("Class not found");

            var students = await studentRepository.GetAll(s => s.ClassId == classId);
            var studentDtos = students.Select(s => mapper.Map<StudentDTO>(s)).ToList();
            return Response<List<StudentDTO>>.Ok(studentDtos);
        }
        catch (InvalidOperationException)
        {
            return Response<List<StudentDTO>>.Fail("Invalid operation while fetching students", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<List<StudentDTO>>.Fail("An unexpected error occurred while fetching students");
        }
    }


}

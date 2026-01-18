using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class ClassService(IRepository<Class> classRepository, IRepository<Student> studentRepository, IMapper mapper) : IClassService
{
    public async Task<Response<ClassDto>> GetClassById(int classId)
    {
        try
        {
            var query = classRepository.Include(c => c.Students).Where(c => c.Id == classId);
            var classEntity = await classRepository.FirstOrDefaultAsync(query);

            if (classEntity == null)
                return Response<ClassDto>.NotFound("Class not found");

            var classDto = mapper.Map<ClassDto>(classEntity);
            return Response<ClassDto>.Ok(classDto);
        }
        catch (InvalidOperationException)
        {
            return Response<ClassDto>.Fail("Invalid operation while fetching class", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<ClassDto>.Fail("An unexpected error occurred while fetching the class");
        }
    }

    public async Task<Response<List<ClassDto>>> GetAllClasses()
    {
        try
        {
            var classes = await classRepository.GetAll();
            var classDtos = classes.Select(c => mapper.Map<ClassDto>(c)).ToList();
            return Response<List<ClassDto>>.Ok(classDtos);
        }
        catch (InvalidOperationException)
        {
            return Response<List<ClassDto>>.Fail("Invalid operation while fetching classes", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<List<ClassDto>>.Fail("An unexpected error occurred while fetching classes");
        }
    }

    public async Task<Response<List<StudentDto>>> GetStudentsByClassId(int classId)
    {
        try
        {
            var classEntity = await classRepository.Get(classId);
            if (classEntity == null)
                return Response<List<StudentDto>>.NotFound("Class not found");

            var students = await studentRepository.GetAll(s => s.ClassId == classId);
            var studentDtos = students.Select(s => mapper.Map<StudentDto>(s)).ToList();
            return Response<List<StudentDto>>.Ok(studentDtos);
        }
        catch (InvalidOperationException)
        {
            return Response<List<StudentDto>>.Fail("Invalid operation while fetching students", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<List<StudentDto>>.Fail("An unexpected error occurred while fetching students");
        }
    }


}

using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IImportService<T>
{
    Task<Response<CourseDto>> Import(T data);
}
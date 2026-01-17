using Domain.Models;

namespace Core.Interfaces.Adapters;

public interface IImportAdapter<T>
{
    public Course GetMappedCourseData(T data);
}
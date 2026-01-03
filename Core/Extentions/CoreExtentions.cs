using Core.Interfaces.Services;
using Core.Mappers;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extentions;
public static class CoreExtentions
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<ICourseService, CourseService>();
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
    }
}

using Core.DocumentGenerator.Factories;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Mappers;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Core.Extentions;
public static class CoreExtentions
{
    public static void AddCoreStartUp(this IServiceCollection services)
    {

        //licensing
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<IPlanningService, PlanningService>();
        services.AddTransient<IDocumentFactory, DocumentFactory>();
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
    }
}

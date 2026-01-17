using Core.DocumentGenerator.Factories;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.Import.Nijmegen;
using Core.Interfaces;
using Core.Interfaces.Adapters;
using Core.Interfaces.Services;
using Core.Mappers;
using Core.Services;
using Core.DTOs.Imports.Nijmegen;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Core.Extensions;
public static class CoreExtensions
{
    public static void AddCoreStartUp(this IServiceCollection services)
    {

        //licensing
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<ILearningOutcomeService, LearningOutcomeService>();
        services.AddTransient<IRubricService, RubricService>();
        services.AddTransient<IValidatorService, ValidatorService>();
        services.AddTransient<IPlanningService, PlanningService>();
        services.AddTransient<IDocumentFactory, DocumentFactory>();
        services.AddTransient<IMaterialService, MaterialService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IClassService, ClassService>();
        services.AddTransient<IStudentService, StudentService>();
        services.AddScoped<IImportAdapter<NijmegenImportDataDto>, NijmegenImport>();
        services.AddScoped<IImportService<NijmegenImportDataDto>, ImportService<NijmegenImportDataDto>>();
        services.AddTransient<IGradeService, GradeService>();
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
    }
}

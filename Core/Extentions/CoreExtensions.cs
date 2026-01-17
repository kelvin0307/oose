using Core.DocumentGenerator.Factories;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.Interfaces.Services;
using Core.Mappers;
using Core.Services;
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
        services.AddTransient<IGradeService, GradeService>();
        services.AddTransient<ILessonService, LessonService>();
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
    }
}

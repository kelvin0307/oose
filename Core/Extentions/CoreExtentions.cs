using Core.DocumentGenerator.Factories;
using Core.Interfaces;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extentions;
public static class CoreExtentions
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IPlanningService, PlanningService>();
        services.AddTransient<IDocumentFactory, DocumentFactory>();
    }








}

using Core.Interfaces.Repositories;
using Data.Context;
using Data.Extensions.ExtensionModels;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions;
public static class DataExtensions
{
    public static void AddDataServices(this IServiceCollection services, ApplicationOptions applicationOptions)
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(applicationOptions.ConnectionString);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IRubricRepository), typeof(RubricRepository));
    }
}

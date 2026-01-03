using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Repositories;
using Data.Context;
using Data.Extentions.ExtentionModels;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extentions;
public static class DataExtentions
{
    public static void AddDataServices(this IServiceCollection services, ApplicationOptions applicationOptions)
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(applicationOptions.ConnectionString);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}

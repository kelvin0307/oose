using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Repositories;
using Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extentions;
public static class DataExtentions
{
    public static void AddDataServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
} 

// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.AspNetCore.Http;
// using Data.Interfaces;
// using Domain.Models;
//
// namespace Server.Extensions
// {
//     public static class ServiceCollectionExtensions
//     {
//         public static IServiceCollection AddHeaderBasedAdapters(this IServiceCollection services)
//         {
//             // HttpContextAccessor nodig om headers te lezen
//             services.AddHttpContextAccessor();
//
//             // ------------------------------
//             // Runtime DI per model
//             // ------------------------------
//
//             // Course
//             services.AddScoped<IDataSource<Course>>(sp =>
//             {
//                 var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
//                 var schoolHeader = httpContext?.Request.Headers["X-Hogeschool"].ToString();
//
//                 return schoolHeader switch
//                 {
//                     "Nijmegen" => sp.GetRequiredService<NijmegenCourseAdapter>(),
//                     // TODO: voeg hier andere hogescholen toe
//                     _ => throw new Exception("Ongeldige of ontbrekende X-Hogeschool header")
//                 };
//             });
//
//             // Lesson
//             services.AddScoped<IDataSource<Lesson>>(sp =>
//             {
//                 var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
//                 var schoolHeader = httpContext?.Request.Headers["X-Hogeschool"].ToString();
//
//                 return schoolHeader switch
//                 {
//                     "Nijmegen" => sp.GetRequiredService<NijmegenLessonAdapter>(),
//                     // TODO: andere hogescholen
//                     _ => throw new Exception("Ongeldige of ontbrekende X-Hogeschool header")
//                 };
//             });
//
//             // LearningOutcome
//             services.AddScoped<IDataSource<LearningOutcome>>(sp =>
//             {
//                 var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
//                 var schoolHeader = httpContext?.Request.Headers["X-Hogeschool"].ToString();
//
//                 return schoolHeader switch
//                 {
//                     "Nijmegen" => sp.GetRequiredService<NijmegenLearningOutcomeAdapter>(),
//                     // TODO: andere hogescholen
//                     _ => throw new Exception("Ongeldige of ontbrekende X-Hogeschool header")
//                 };
//             });
//
//             return services;
//         }
//     }
// }

using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extentions.ModelExtentions
{
    public static class LearningOutcomeExtensions
    {
        public static IQueryable<LearningOutcome> FindByCourseId(this IQueryable<LearningOutcome> learningOutcomes, int courseId)
        {
            return learningOutcomes.Where(x => x.CourseId == courseId);
        }
    }
}

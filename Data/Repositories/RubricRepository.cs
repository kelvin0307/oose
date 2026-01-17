using Data.Interfaces.Repositories;
using Data.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class RubricRepository : Repository<Rubric>, IRubricRepository
    {
        private readonly DataContext _context;

        public RubricRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Loads the full Rubric aggregate:
        /// Rubric -> AssessmentDimensions -> AssessmentDimensionScores
        /// </summary>
        public async Task<Rubric?> GetAggregate(int id)
        {
            return await _context.Set<Rubric>()
                .Include(r => r.AssessmentDimensions)
                .ThenInclude(d => d.AssessmentDimensionScores)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        
        public async Task<List<Rubric>> GetAggregatesByLearningOutcomeId(int learningOutcomeId)
        {
            return await _context.Set<Rubric>()
                .Include(r => r.AssessmentDimensions)
                .ThenInclude(d => d.AssessmentDimensionScores)
                .Where(r => r.LearningOutcomeId == learningOutcomeId)
                .ToListAsync();
        }
        
        public async Task SaveAggregate()
        {
            await _context.SaveChangesAsync();
        }
    }
}
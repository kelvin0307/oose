using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class DataContext : DbContext
    {
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Planning> Plannings { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<LearningOutcome> LearningOutcomes { get; set; }
        public virtual DbSet<CourseExecution> CourseExecutions { get; set; }
        public virtual DbSet<AssessmentDimension> AssessmentDimensions { get; set; }
        public virtual DbSet<AssessmentDimensionScore> AssessmentDimensionScores { get; set; }
        public virtual DbSet<Rubric> Rubrics { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Planning)
                .WithOne(p => p.Course)
                .HasForeignKey<Planning>(p => p.CourseId);
            modelBuilder.Entity<Course>()
                .HasMany(c => c.LearningOutcomes)
                .WithOne(lo => lo.Course)
                .HasForeignKey(lo => lo.CourseId);
            modelBuilder.Entity<Lesson>()
                .HasMany(x => x.Materials);
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Planning)
                .WithMany(p => p.Lessons)
                .HasForeignKey(l => l.PlanningId);
            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.LearningOutcomes)
                .WithMany(lo => lo.Lessons)
                .UsingEntity<Dictionary<string, object>>(
                    "LessonLearningOutcome",
                    j => j.HasOne<LearningOutcome>()
                          .WithMany()
                          .HasForeignKey("LessonId")
                          .OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<Lesson>()
                          .WithMany()
                          .HasForeignKey("LearningOutcomeId")
                          .OnDelete(DeleteBehavior.Cascade)
                );
            modelBuilder.Entity<LearningOutcome>()
                .HasMany(lo => lo.Rubrics)
                .WithOne(r => r.LearningOutcome)
                .HasForeignKey(r => r.LearningOutcomeId);
            modelBuilder.Entity<Rubric>()
                .HasMany(r => r.AssessmentDimensions)
                .WithOne(ad => ad.Rubric)
                .HasForeignKey(ad => ad.RubricId);
            modelBuilder.Entity<AssessmentDimension>()
                .HasMany(ad => ad.AssessmentDimensionScores)
                .WithOne(ads => ads.AssessmentDimension)
                .HasForeignKey(ads => ads.AssessmentDimensionId);
            modelBuilder.Entity<Material>()
                .HasKey(m => new { m.Id, m.Version });
            modelBuilder.Entity<CourseExecution>()
                .HasMany(ce => ce.Classes)
                .WithOne(c => c.CourseExecution)
                .HasForeignKey(c => c.CourseExecutionId);
            modelBuilder.Entity<Class>()
                .HasMany(c => c.Students)
                .WithOne(s => s.Class)
                .HasForeignKey(s => s.ClassId);
            modelBuilder.Entity<CourseExecution>()
                .HasMany(ce => ce.Grades)
                .WithOne(g => g.CourseExecution)
                .HasForeignKey(x => x.CourseExcecutionId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
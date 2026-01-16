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
                .HasMany(lo => lo.LearningOutcomes)
                .WithMany(l => l.Lessons)
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
            modelBuilder.Entity<Material>()
                .HasKey(x => new { x.Id, x.Version });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

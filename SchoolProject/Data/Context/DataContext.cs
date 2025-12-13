using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context
{
    public class DataContext : DbContext
    {
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Planning> Plannings { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
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
                .HasOne(l => l.Planning)
                .WithMany(p => p.Lessons)
                .HasForeignKey(l => l.PlanningId);
            modelBuilder.Entity<LearningOutcome>()
                .HasMany(lo => lo.Lessons)
                .WithMany(l => l.LearningOutcomes)
                .UsingEntity<Dictionary<string, object>>(
                    "LessonLearningOutcome",
                    j => j.HasOne<Lesson>().WithMany().HasForeignKey("LessonId"),
                    j => j.HasOne<LearningOutcome>().WithMany().HasForeignKey("LearningOutcomeId") 
                );
        }
    }
}

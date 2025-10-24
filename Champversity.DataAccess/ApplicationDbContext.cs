using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Champversity.DataAccess.Models;

namespace Champversity.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<InterviewSlot> InterviewSlots { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<ValidationRule> ValidationRules { get; set; }
        public DbSet<UniversityResponse> UniversityResponses { get; set; }
        public DbSet<ManualTask> ManualTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<InterviewSlot>()
         .HasOne<Student>()
    .WithMany(s => s.InterviewSlots)
                .HasForeignKey(s => s.StudentId);

            modelBuilder.Entity<Certification>()
                .HasOne<Student>()
                .WithMany(s => s.Certifications)
                .HasForeignKey(c => c.StudentId);

            modelBuilder.Entity<Achievement>()
                .HasOne<Student>()
                .WithMany(s => s.Achievements)
                .HasForeignKey(a => a.StudentId);

            modelBuilder.Entity<Volunteer>()
                .HasOne<Student>()
                .WithMany(s => s.Volunteers)
                .HasForeignKey(v => v.StudentId);

            modelBuilder.Entity<ManualTask>()
  .HasOne<Student>()
       .WithMany()
          .HasForeignKey(m => m.StudentId);

            modelBuilder.Entity<UniversityResponse>()
    .HasOne<Student>()
    .WithMany()
         .HasForeignKey(u => u.StudentId);
        }
 }
}
using Microsoft.EntityFrameworkCore;
using Champversity.DataAccess.Models;

namespace Champversity.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<InterviewSlot> InterviewSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        base.OnModelCreating(modelBuilder);

          // Configure relationships
            modelBuilder.Entity<InterviewSlot>()
         .HasOne<Student>()
    .WithMany(s => s.InterviewSlots)
                .HasForeignKey(s => s.StudentId);
        }
    }
}
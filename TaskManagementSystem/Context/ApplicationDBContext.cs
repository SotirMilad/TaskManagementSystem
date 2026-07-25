using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;


namespace TaskManagementSystem.Context
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);

                // Duplicate names are rejected 
                entity.HasIndex(p => p.Name).IsUnique();
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.Property(t => t.Title).IsRequired().HasMaxLength(300);

                //Deleting a project must cascade delete
                entity.HasOne(t => t.Project)
                      .WithMany(p => p.Tasks)
                      .HasForeignKey(t => t.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(t => t.ProjectId);
                entity.HasIndex(t => t.Status);
                entity.HasIndex(t => t.Priority);
                entity.HasIndex(t => t.DueDate);
                entity.HasIndex(t => t.CreatedAt);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}

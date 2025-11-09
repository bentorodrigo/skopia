using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Db;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<TaskHistory> Histories => Set<TaskHistory>();
    public DbSet<Comment> Comments => Set<Comment>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<User>()
            .HasMany(u => u.Projects)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<User>().Property(p => p.Id).ValueGeneratedOnAdd();

        modelBuilder
            .Entity<Project>()
            .HasMany(p => p.TaskItems)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId);

        modelBuilder.Entity<Project>().Property(p => p.Id).ValueGeneratedOnAdd();

        modelBuilder
            .Entity<TaskItem>()
            .HasMany(t => t.Historic)
            .WithOne(h => h.TaskItem)
            .HasForeignKey(h => h.TaskItemId);

        modelBuilder.Entity<TaskItem>().Property(p => p.Id).ValueGeneratedOnAdd();

        modelBuilder
            .Entity<TaskItem>()
            .HasMany(t => t.Comments)
            .WithOne(c => c.TaskItem)
            .HasForeignKey(c => c.TaskItemId);

        modelBuilder.Entity<TaskHistory>().Property(p => p.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Comment>().Property(p => p.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Rodrigo Bento", Email = "rodrigo@example.com", Function = Domain.Enums.UserFunction.Gerente },
            new User { Id = 2, Name = "Maria Silva", Email = "maria@example.com", Function = Domain.Enums.UserFunction.Normal },
            new User { Id = 3, Name = "Carlos Souza", Email = "carlos@example.com", Function = Domain.Enums.UserFunction.Normal }
        );
    }
}
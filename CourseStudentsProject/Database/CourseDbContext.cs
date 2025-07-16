using CourseStudentsProject.Database.Configurations;
using CourseStudentsProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseStudentsProject.Database;

public class CourseDbContext(DbContextOptions<CourseDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
using CourseStudentsProject.Database;
using CourseStudentsProject.DTO;
using CourseStudentsProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseStudentsProject.Controllers;

[ApiController]
[Route("[controller]")]
public class CoursesController(ILogger<CoursesController> logger, IDbContextFactory<CourseDbContext> context)
    : ControllerBase
{
    private readonly ILogger<CoursesController> _logger = logger;
    private readonly IDbContextFactory<CourseDbContext> _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCourseResponse>>> GetCourses()
    {
        _logger.LogInformation("GetCourses");
        await using var context = await _context.CreateDbContextAsync();
        var courses = await context.Courses
            .Include(c => c.Students)
            .ToListAsync();

        var request = courses.Select(c => new GetCourseResponse
            {
                Id = c.Id,
                Name = c.Name,
                Students = c.Students.Select(s => new GetStudentResponse
                    {
                        Id = s.Id,
                        Name = s.Name
                    }
                )
            }
        );

        _logger.LogInformation("GetCourses");
        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<CourseRequest>>> CreateCourse(CourseRequest request)
    {
        await using var context = await _context.CreateDbContextAsync();
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();
        _logger.LogInformation("CreateCourse");

        return Ok(new CourseResponse { Id = course.Id });
    }

    [HttpPost("{id:guid}/students")]
    public async Task<ActionResult<StudentResponse>> AddStudent(Guid id, StudentRequest request)
    {
        await using var context = await _context.CreateDbContextAsync();
        var course = await context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CourseId = id
        };
        
        context.Students.Add(student);
        await context.SaveChangesAsync();
        _logger.LogInformation("AddStudent");
        
        return Ok(new StudentResponse { Id = student.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCourse(Guid id)
    {
        await using var context = await _context.CreateDbContextAsync();
        var course = await context.Courses.FindAsync(id);
        if (course is null) return NotFound();

        context.Courses.Remove(course);
        await context.SaveChangesAsync();
        _logger.LogInformation("DeleteCourse");

        return NoContent();
    }
}
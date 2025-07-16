namespace CourseStudentsProject.DTO;

public class GetCourseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<GetStudentResponse> Students { get; set; }
}
namespace SchoolDisciplineApp.Application.DTOs;
public class AttendanceRecordDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public DateTime Date { get; set; }
    public bool IsAbsent { get; set; }
    public bool? IsExcused { get; set; }
}


namespace SchoolDisciplineApp.Application.DTOs;
public class ReportDto
{
    public int Id { get; set; }
    public string ReportType { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string ReportData { get; set; }
}

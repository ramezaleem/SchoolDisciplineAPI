namespace SchoolDisciplineApp.Application.DTOs;
using SchoolDisciplineApp.Domain.Enums;

public class BehaviorRecordDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public BehaviorType BehaviorType { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}


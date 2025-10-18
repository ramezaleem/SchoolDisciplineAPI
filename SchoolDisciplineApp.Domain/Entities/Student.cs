using SchoolDisciplineApp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class Student
{
    public int Id { get; set; }

    [Required(ErrorMessage = "يجب إدخال اسم الطالب.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "اسم الطالب يجب أن يكون بين 3 و50 حرف.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "يجب تحديد الصف الدراسي للطالب.")]
    public int ClassId { get; set; }  // فقط Id

    [RegularExpression(@"^[a-zA-Z0-9]{1,10}$", ErrorMessage = "رقم الجلوس يجب أن يكون بين 1 و10 أحرف أو أرقام.")]
    public string? RollNumber { get; set; }

    // EF Core سيملأها تلقائياً من الـ ClassId
    public SchoolClass Class { get; set; }

    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<BehaviorRecord> BehaviorRecords { get; set; } = new List<BehaviorRecord>();
}

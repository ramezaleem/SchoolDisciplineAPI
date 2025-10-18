using System.ComponentModel.DataAnnotations;

public class SchoolClass
{
    public int Id { get; set; }

    [Required(ErrorMessage = "يجب إدخال الفصل الدراسي.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "اسم الفصل الدراسي يجب أن يكون بين 3 و20 حرف.")]
    public string AcademicTerm { get; set; }

    [Required(ErrorMessage = "يجب إدخال اسم الصف.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "اسم الصف يجب أن يكون بين 2 و20 حرف.")]
    public string ClassName { get; set; }

    [Range(1, 50, ErrorMessage = "عدد الطلاب يجب أن يكون بين 1 و50.")]
    public int StudentCount { get; set; }

    [StringLength(100, ErrorMessage = "اسم المراقب يجب أن لا يتجاوز 100 حرف.")]
    public string Director { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}

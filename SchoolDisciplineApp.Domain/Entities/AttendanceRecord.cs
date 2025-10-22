using System.ComponentModel.DataAnnotations;

namespace SchoolDisciplineApp.Domain.Entities
{
    public class AttendanceRecord
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار الطالب.")]
        [Range(1, int.MaxValue, ErrorMessage = "معرّف الطالب غير صالح.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الصف الدراسي.")]
        [Range(1, int.MaxValue, ErrorMessage = "معرّف الصف الدراسي غير صالح.")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "يجب تحديد تاريخ الحضور.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(AttendanceRecord), nameof(ValidateDate))]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "يجب تحديد حالة الغياب.")]
        public bool IsAbsent { get; set; }  // true = غائب، false = حاضر

        [Required(ErrorMessage = "يجب تحديد إذا كان الغياب بعذر أو بدون عذر.")]
        public bool? IsExcused { get; set; }  // true = بعذر، false = بدون عذر، null إذا لم يكن غائباً

        public Student Student { get; set; }
        public SchoolClass Class { get; set; }

        public static ValidationResult ValidateDate ( DateTime date, ValidationContext context )
        {
            return date > DateTime.Today
                ? new ValidationResult("تاريخ الحضور لا يمكن أن يكون في المستقبل.")
                : ValidationResult.Success;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SchoolDisciplineApp.Domain.Entities
{
    public class Report
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب تحديد نوع التقرير.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "نوع التقرير يجب أن يكون بين 3 و50 حرف.")]
        public string ReportType { get; set; }

        [Required(ErrorMessage = "يجب تحديد تاريخ إنشاء التقرير.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Report), nameof(ValidateGeneratedAt))]
        public DateTime GeneratedAt { get; set; }

        [Required(ErrorMessage = "يجب إدخال بيانات التقرير.")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "بيانات التقرير يجب أن تكون بين 10 و5000 حرف.")]
        public string ReportData { get; set; }

        // تحقق أن التاريخ ليس في المستقبل
        public static ValidationResult ValidateGeneratedAt ( DateTime date, ValidationContext context )
        {
            if (date > DateTime.Today)
                return new ValidationResult("تاريخ إنشاء التقرير لا يمكن أن يكون في المستقبل.");
            return ValidationResult.Success;
        }
    }
}

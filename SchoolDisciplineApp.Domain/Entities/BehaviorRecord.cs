using SchoolDisciplineApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolDisciplineApp.Domain.Entities
{
    public class BehaviorRecord
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار الطالب.")]
        [Range(1, int.MaxValue, ErrorMessage = "معرّف الطالب غير صالح.")]
        public int StudentId { get; set; }  // فقط Id

        [Required(ErrorMessage = "يجب تحديد نوع السلوك.")]
        [EnumDataType(typeof(BehaviorType), ErrorMessage = "نوع السلوك غير صالح.")]
        public BehaviorType BehaviorType { get; set; }

        [Required(ErrorMessage = "يجب كتابة وصف للسلوك.")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "يجب أن يكون الوصف بين 5 و 300 حرف.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "يجب تحديد تاريخ السلوك.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(BehaviorRecord), nameof(ValidateDate))]
        public DateTime Date { get; set; }

        public Student Student { get; set; }  // EF Core يربطها من الـ StudentId

        public static ValidationResult ValidateDate ( DateTime date, ValidationContext context )
        {
            return date > DateTime.Today ? new ValidationResult("تاريخ السلوك لا يمكن أن يكون في المستقبل.") : ValidationResult.Success;
        }
    }

}

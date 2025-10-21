using Microsoft.AspNetCore.Mvc;
using SchoolDisciplineApp.Application.DTOs;
using SchoolDisciplineApp.Application.Services.Interfaces;

namespace SchoolDisciplineApp.API.Controllers
{
    /// <summary>
    /// Manages class-related operations such as retrieving, adding, updating, and deleting classes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly IStudentService _studentService;
        private readonly IAttendanceService _attendanceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassController"/> class.
        /// </summary>
        /// <param name="classService">The service responsible for handling class operations.</param>
        /// <param name="studentService">The service responsible for handling student operations related to classes.</param>
        /// <param name="attendanceService">The service responsible for managing attendance records related to students in classes.</param>
        public ClassController ( IClassService classService, IStudentService studentService, IAttendanceService attendanceService )
        {
            _classService = classService;
            _studentService = studentService;
            _attendanceService = attendanceService;
        }


        /// <summary>
        /// Retrieves all school classes.
        /// </summary>
        /// <returns>A list of all classes.</returns>
        /// <response code="200">Returns all classes successfully.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll ()
        {
            var classes = await _classService.GetAllAsync();
            var dtos = classes.Select(x => new SchoolClassDto
            {
                Id = x.Id,
                AcademicTerm = x.AcademicTerm,
                ClassName = x.ClassName,
                StudentCount = x.StudentCount,
                Director = x.Director
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Retrieves a specific class by its unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the class.</param>
        /// <returns>The class details if found; otherwise, a NotFound response.</returns>
        /// <response code="200">Returns the class details successfully.</response>
        /// <response code="404">If no class is found with the given ID.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById ( int id )
        {
            var schoolClass = await _classService.GetByIdAsync(id);
            if (schoolClass == null)
                return NotFound(new { message = $"لم يتم العثور على الصف بالمعرّف {id}." });

            var dto = new SchoolClassDto
            {
                Id = schoolClass.Id,
                AcademicTerm = schoolClass.AcademicTerm,
                ClassName = schoolClass.ClassName,
                StudentCount = schoolClass.StudentCount,
                Director = schoolClass.Director
            };

            return Ok(dto);
        }

        /// <summary>
        /// Adds a new school class.
        /// </summary>
        /// <param name="dto">The DTO object for the class to be added.</param>
        /// <returns>The newly created class object.</returns>
        /// <response code="201">Class added successfully.</response>
        /// <response code="400">If the input data is invalid.</response>
        [HttpPost]
        public async Task<IActionResult> Add ( [FromBody] SchoolClassDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "البيانات المدخلة غير صحيحة. يرجى التحقق من القيم المُرسلة." });

            var schoolClass = new SchoolClass
            {
                AcademicTerm = dto.AcademicTerm,
                ClassName = dto.ClassName,
                StudentCount = dto.StudentCount,
                Director = dto.Director
            };

            await _classService.AddAsync(schoolClass);
            return CreatedAtAction(nameof(GetById), new { id = schoolClass.Id }, new
            {
                message = "تمت إضافة الصف الدراسي بنجاح.",
                schoolClass
            });
        }

        /// <summary>
        /// Updates an existing class information.
        /// </summary>
        /// <param name="id">The unique ID of the class to update.</param>
        /// <param name="dto">The updated class DTO data.</param>
        /// <response code="204">Class updated successfully.</response>
        /// <response code="400">If the input data is invalid.</response>
        /// <response code="404">If no class is found with the given ID.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update ( int id, [FromBody] SchoolClassDto dto )
        {
            if (id != dto.Id)
                return BadRequest(new { message = "معرّف الصف المرسل لا يتطابق مع المعرّف المطلوب تعديله." });

            var existingClass = await _classService.GetByIdAsync(id);
            if (existingClass == null)
                return NotFound(new { message = $"لم يتم العثور على الصف بالمعرّف {id}." });

            existingClass.AcademicTerm = dto.AcademicTerm;
            existingClass.ClassName = dto.ClassName;
            existingClass.StudentCount = dto.StudentCount;
            existingClass.Director = dto.Director;

            await _classService.UpdateAsync(existingClass);
            return Ok(new { message = "تم تحديث بيانات الصف الدراسي بنجاح." });
        }

        /// <summary>
        /// Deletes a specific class by ID.
        /// </summary>
        /// <param name="id">The unique ID of the class to delete.</param>
        /// <response code="204">Class deleted successfully.</response>
        /// <response code="404">If no class is found with the given ID.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var existingClass = await _classService.GetByIdAsync(id);
            if (existingClass == null)
                return NotFound(new { message = $"لم يتم العثور على الصف بالمعرّف {id}." });

            var relatedStudents = await _classService.GetStudentsByClassIdAsync(id);

            if (relatedStudents != null && relatedStudents.Any())
            {
                foreach (var student in relatedStudents)
                {
                    var attendanceRecords = await _attendanceService.GetByStudentIdAsync(student.Id);
                    if (attendanceRecords != null && attendanceRecords.Any())
                    {
                        foreach (var record in attendanceRecords)
                        {
                            await _attendanceService.DeleteAsync(record.Id);
                        }
                    }

                    await _studentService.DeleteAsync(student.Id);
                }
            }

            await _classService.DeleteAsync(id);

            return Ok(new { message = "تم حذف الصف وجميع الطلاب وسجلات الحضور المرتبطة بهم بنجاح." });
        }


    }
}

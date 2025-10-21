using Microsoft.AspNetCore.Mvc;
using SchoolDisciplineApp.Application.DTOs;
using SchoolDisciplineApp.Application.Services.Interfaces;

namespace SchoolDisciplineApp.API.Controllers
{
    /// <summary>
    /// Manages student-related operations such as retrieving, adding, updating, and deleting students.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IAttendanceService _attendanceService;
        private readonly IExcelImportService _excelImportService;
        private readonly IClassService _classService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentController"/> class.
        /// </summary>
        /// <param name="studentService">The service responsible for handling student operations.</param>
        /// <param name="attendanceService">The service responsible for managing attendance records.</param>
        /// <param name="excelImportService">The service responsible for processing Excel file imports.</param>
        /// <param name="classService">The service responsible for managing class operations.</param>
        public StudentController (
            IStudentService studentService,
            IAttendanceService attendanceService,
            IExcelImportService excelImportService,
            IClassService classService )
        {
            _studentService = studentService;
            _attendanceService = attendanceService;
            _excelImportService = excelImportService;
            _classService = classService;
        }


        /// <summary>
        /// Retrieves all students from the system or filters by class if classId is provided.
        /// </summary>
        /// <param name="classId">Optional. The ID of the class to filter students.</param>
        /// <returns>A list of students, optionally filtered by class.</returns>
        /// <response code="200">Returns the list of students successfully.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll ( [FromQuery] int? classId = null )
        {
            var students = await _studentService.GetAllAsync();

            if (classId.HasValue)
                students = students.Where(s => s.ClassId == classId.Value);

            var studentDtos = students.Select(s => new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                ClassId = s.ClassId,
            });

            return Ok(studentDtos);
        }

        /// <summary>
        /// Retrieves a specific student by their unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the student.</param>
        /// <returns>The student details if found; otherwise, a NotFound response.</returns>
        /// <response code="200">Returns the student details successfully.</response>
        /// <response code="404">If no student is found with the given ID.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById ( int id )
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound(new { message = $"لم يتم العثور على الطالب بالمعرّف {id}." });

            var dto = new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                ClassId = student.ClassId,
            };

            return Ok(dto);
        }

        /// <summary>
        /// Adds a new student or a list of students to the system.
        /// </summary>
        /// <param name="dtos">Single student DTO or a list of student DTOs.</param>
        /// <returns>Result of the addition operation.</returns>
        /// <response code="201">Student(s) added successfully.</response>
        /// <response code="400">If the input data is invalid or referenced class does not exist.</response>
        [HttpPost]
        public async Task<IActionResult> Add ( [FromBody] List<CreateStudentDto> dtos )
        {
            if (dtos == null || dtos.Count == 0)
                return BadRequest(new { message = "البيانات المدخلة غير صحيحة أو فارغة. يرجى التحقق من القيم المُرسلة." });

            var addedStudents = new List<StudentDto>();

            foreach (var dto in dtos)
            {
                if (!TryValidateModel(dto))
                    return BadRequest(new { message = $"بيانات الطالب {dto.Name} غير صحيحة." });

                // تحقق من وجود الصف
                var classExists = await _classService.GetByIdAsync(dto.ClassId);
                if (classExists == null)
                {
                    return BadRequest(new { message = $"الصف الذي يحمل المعرف {dto.ClassId} غير موجود." });
                }

                var student = new Student
                {
                    Name = dto.Name,
                    ClassId = dto.ClassId,
                    RollNumber = ""
                };

                var createdStudent = await _studentService.AddAsync(student);

                addedStudents.Add(new StudentDto
                {
                    Id = createdStudent.Id,
                    Name = createdStudent.Name,
                    ClassId = createdStudent.ClassId
                });
            }

            return Created("", new
            {
                message = $"تمت إضافة {addedStudents.Count} طالباً بنجاح.",
                students = addedStudents
            });
        }


        /// <summary>
        /// Imports multiple students in bulk.
        /// </summary>
        /// <remarks>
        /// Example of request body:
        /// 
        /// [
        ///   { "name": "أحمد علي", "classId": 1, "rollNumber": "A01" },
        ///   { "name": "سارة محمد", "classId": 1, "rollNumber": "A02" }
        /// ]
        /// </remarks>
        /// <param name="dtos">List of student DTOs to import.</param>
        /// <returns>A message indicating how many students were successfully imported.</returns>
        /// <response code="200">Returns the number of students successfully imported.</response>
        /// <response code="400">If the input data is null, empty, or any student data is invalid.</response>
        [HttpPost("import")]
        public async Task<IActionResult> Import ( [FromBody] IEnumerable<CreateStudentDto> dtos )
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { message = "لا توجد بيانات طلابية صالحة للاستيراد." });

            foreach (var dto in dtos)
            {
                if (!TryValidateModel(dto))
                    return BadRequest(new { message = $"بيانات الطالب {dto.Name} غير صحيحة." });

                var student = new Student
                {
                    Name = dto.Name,
                    ClassId = dto.ClassId,
                };

                await _studentService.AddAsync(student);
            }

            return Ok(new { message = $"تم استيراد {dtos.Count()} طالباً بنجاح." });
        }

        /// <summary>
        /// Updates an existing student's information.
        /// </summary>
        /// <param name="id">The unique ID of the student to update.</param>
        /// <param name="dto">The updated student DTO data.</param>
        /// <response code="204">Student updated successfully.</response>
        /// <response code="400">If the input data is invalid.</response>
        /// <response code="404">If no student is found with the given ID.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update ( int id, [FromBody] CreateStudentDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "البيانات المدخلة غير صحيحة. يرجى التحقق من القيم المُرسلة." });

            var existingStudent = await _studentService.GetByIdAsync(id);
            if (existingStudent == null)
                return NotFound(new { message = $"لم يتم العثور على الطالب بالمعرّف {id}." });

            existingStudent.Name = dto.Name;
            existingStudent.ClassId = dto.ClassId;

            await _studentService.UpdateAsync(existingStudent);
            return Ok(new { message = "تم تحديث بيانات الطالب بنجاح." });
        }

        /// <summary>
        /// Uploads and processes multiple Excel files containing student data.
        /// </summary>
        /// <param name="files">A list of Excel files to be uploaded and processed.</param>
        /// <response code="200">Files processed and data imported successfully.</response>
        /// <response code="400">If no files are provided for upload.</response>
        [HttpPost("upload-excel-files")]
        public async Task<IActionResult> UploadExcelFiles ( List<IFormFile> files )
        {
            if (files == null || !files.Any())
                return BadRequest("لا توجد ملفات للرفع.");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    await _excelImportService.ProcessExcelFileAsync(stream);
                }
            }
            return Ok("تم استيراد الملفات بنجاح.");
        }

        /// <summary>
        /// Deletes a specific student from the system by ID.
        /// </summary>
        /// <param name="id">The unique ID of the student to delete.</param>
        /// <response code="204">Student deleted successfully.</response>
        /// <response code="404">If no student is found with the given ID.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var existingStudent = await _studentService.GetByIdAsync(id);
            if (existingStudent == null)
                return NotFound(new { message = $"لم يتم العثور على الطالب بالمعرّف {id}." });

            var attendanceRecords = await _attendanceService.GetByStudentIdAsync(id);
            if (attendanceRecords != null && attendanceRecords.Any())
            {
                foreach (var record in attendanceRecords)
                {
                    await _attendanceService.DeleteAsync(record.Id);
                }
            }

            await _studentService.DeleteAsync(id);

            return Ok(new { message = "تم حذف الطالب بنجاح." });
        }



    }
}

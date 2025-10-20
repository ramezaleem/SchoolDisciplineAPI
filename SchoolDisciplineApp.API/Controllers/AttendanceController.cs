using Microsoft.AspNetCore.Mvc;
using SchoolDisciplineApp.Application.DTOs;
using SchoolDisciplineApp.Application.Services.Interfaces;

namespace SchoolDisciplineApp.API.Controllers
{
    /// <summary>
    /// Manages student attendance records (CRUD operations).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IStudentService _studentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttendanceController"/> class.
        /// </summary>
        /// <param name="attendanceService">Service responsible for managing attendance records.</param>
        /// <param name="studentService">Service responsible for managing student records.</param>
        public AttendanceController ( IAttendanceService attendanceService, IStudentService studentService )
        {
            _attendanceService = attendanceService;
            _studentService = studentService;
        }

        /// <summary>
        /// Retrieves all attendance records, optionally filtered by class and month.
        /// </summary>
        /// <param name="classId">Optional: Filter by class ID.</param>
        /// <param name="month">Optional: Filter by month (1-12).</param>
        /// <returns>List of attendance records.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll ( [FromQuery] int? classId = null, [FromQuery] int? month = null )
        {
            var records = await _attendanceService.GetAllAsync();

            if (classId.HasValue)
                records = records.Where(r => r.ClassId == classId.Value);

            if (month.HasValue)
                records = records.Where(r => r.Date.Month == month.Value);

            if (!records.Any())
                return NotFound(new { message = "لا توجد سجلات حضور مطابقة للمعايير المحددة." });

            var dtos = records.Select(x => new AttendanceRecordDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                ClassId = x.ClassId,
                Date = x.Date,
                IsAbsent = x.IsAbsent
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Retrieves a specific attendance record by its unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the attendance record.</param>
        /// <returns>The attendance record details.</returns>
        /// <response code="200">Returns the attendance record successfully.</response>
        /// <response code="404">If no attendance record is found with the given ID.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById ( int id )
        {
            var record = await _attendanceService.GetByIdAsync(id);
            if (record == null)
                return NotFound(new { message = $"لم يتم العثور على سجل الحضور بالمعرّف {id}." });

            var dto = new AttendanceRecordDto
            {
                Id = record.Id,
                StudentId = record.StudentId,
                ClassId = record.ClassId,
                Date = record.Date,
                IsAbsent = record.IsAbsent
            };

            return Ok(dto);
        }

        /// <summary>
        /// Retrieves all attendance records for a specific student.
        /// </summary>
        /// <param name="studentId">The unique ID of the student.</param>
        /// <returns>List of attendance records for the student.</returns>
        /// <response code="200">Returns the list of attendance records successfully.</response>
        /// <response code="404">If no records found for the student.</response>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId ( int studentId )
        {
            var records = await _attendanceService.GetByStudentIdAsync(studentId);
            if (records == null || !records.Any())
                return NotFound(new { message = $"لا توجد سجلات حضور للطالب بالمعرّف {studentId}." });

            var dtos = records.Select(x => new AttendanceRecordDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                ClassId = x.ClassId,
                Date = x.Date,
                IsAbsent = x.IsAbsent
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Retrieves all students in a class and their attendance status on a specified date.
        /// Returns students even if there is no attendance record for them on that date (assumed present).
        /// </summary>
        /// <param name="classId">The ID of the class to filter students.</param>
        /// <param name="date">The date to filter attendance records.</param>
        /// <returns>List of students with their attendance status on the specified date.</returns>
        /// <response code="200">Returns the list of students with attendance status.</response>
        /// <response code="404">If no students found for the specified class.</response>
        [HttpGet("class-attendance-by-date")]
        public async Task<IActionResult> GetAttendanceByClassAndDate ( [FromQuery] int classId, [FromQuery] DateTime date )
        {
            var students = await _studentService.GetByClassIdAsync(classId);
            if (students == null || !students.Any())
                return NotFound(new { message = $"لا يوجد طلاب بالفصل رقم {classId}." });

            var attendanceRecords = await _attendanceService.GetForClassByDateAsync(classId, date.Date);

            if (attendanceRecords == null || !attendanceRecords.Any())
                return NotFound(new { message = $"لا يوجد سجلات حضور في التاريخ {date:yyyy-MM-dd} للفصل رقم {classId}." });

            var result = students.Select(student =>
            {
                var att = attendanceRecords.FirstOrDefault(a => a.StudentId == student.Id && a.Date.Date == date.Date);
                return new
                {
                    id = att?.Id ?? 0,
                    studentId = student.Id,
                    name = student.Name,
                    classId = student.ClassId,
                    date = date,
                    isAbsent = att?.IsAbsent ?? false,
                };
            });

            return Ok(result);
        }

        /// <summary>
        /// Adds a new attendance record.
        /// </summary>
        /// <param name="dto">The attendance record DTO object to add.</param>
        /// <returns>The created attendance record.</returns>
        /// <response code="201">Attendance record added successfully.</response>
        /// <response code="400">If the input data is invalid.</response>
        [HttpPost]
        public async Task<IActionResult> Add ( [FromBody] AttendanceRecordDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "البيانات المدخلة غير صحيحة. يرجى التحقق من القيم المُرسلة." });

            var record = new Domain.Entities.AttendanceRecord
            {
                StudentId = dto.StudentId,
                ClassId = dto.ClassId,
                Date = dto.Date,
                IsAbsent = dto.IsAbsent
            };

            await _attendanceService.AddAsync(record);

            dto.Id = record.Id;

            return CreatedAtAction(nameof(GetById), new { id = record.Id }, new
            {
                message = "تمت إضافة سجل الحضور بنجاح.",
                record = dto
            });
        }

        /// <summary>
        /// Updates an existing attendance record.
        /// </summary>
        /// <param name="id">The unique ID of the record to update.</param>
        /// <param name="dto">The updated attendance record DTO data.</param>
        /// <returns>Update result.</returns>
        /// <response code="200">Attendance record updated successfully.</response>
        /// <response code="400">If the input data is invalid.</response>
        /// <response code="404">If no record is found with the given ID.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update ( int id, [FromBody] AttendanceRecordDto dto )
        {
            if (id != dto.Id)
                return BadRequest(new { message = "معرّف سجل الحضور المرسل لا يتطابق مع المعرّف المطلوب تعديله." });

            var existingRecord = await _attendanceService.GetByIdAsync(id);
            if (existingRecord == null)
                return NotFound(new { message = $"لم يتم العثور على سجل الحضور بالمعرّف {id}." });

            existingRecord.StudentId = dto.StudentId;
            existingRecord.ClassId = dto.ClassId;
            existingRecord.Date = dto.Date;
            existingRecord.IsAbsent = dto.IsAbsent;

            await _attendanceService.UpdateAsync(existingRecord);

            return Ok(new { message = "تم تحديث سجل الحضور بنجاح." });
        }


        /// <summary>
        /// Updates multiple attendance records at once.
        /// </summary>
        /// <param name="dtos">A list of attendance record DTOs to be updated.</param>
        /// <returns>Operation result including how many records were successfully updated.</returns>
        /// <response code="200">All valid attendance records updated successfully.</response>
        /// <response code="400">If the input data is invalid or empty.</response>
        /// <response code="404">If none of the provided records were found in the database.</response>
        [HttpPut("update-multiple")]
        public async Task<IActionResult> UpdateMultiple ( [FromBody] List<AttendanceRecordDto> dtos )
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { message = "قائمة سجلات الحضور فارغة أو غير صالحة." });

            int updatedCount = 0;

            foreach (var dto in dtos)
            {
                var existingRecord = await _attendanceService.GetByIdAsync(dto.Id);
                if (existingRecord == null)
                    continue;

                existingRecord.StudentId = dto.StudentId;
                existingRecord.ClassId = dto.ClassId;
                existingRecord.Date = dto.Date;
                existingRecord.IsAbsent = dto.IsAbsent;

                await _attendanceService.UpdateAsync(existingRecord);
                updatedCount++;
            }

            if (updatedCount == 0)
                return NotFound(new { message = "لم يتم العثور على أي سجلات حضور مطابقة للتحديث." });

            return Ok(new { message = $"تم تحديث {updatedCount} سجل حضور بنجاح." });
        }


        /// <summary>
        /// Deletes an attendance record by its ID.
        /// </summary>
        /// <param name="id">The unique ID of the record to delete.</param>
        /// <returns>Delete result.</returns>
        /// <response code="200">Attendance record deleted successfully.</response>
        /// <response code="404">If no record is found with the given ID.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var existingRecord = await _attendanceService.GetByIdAsync(id);
            if (existingRecord == null)
                return NotFound(new { message = $"لم يتم العثور على سجل الحضور بالمعرّف {id}." });

            await _attendanceService.DeleteAsync(id);

            return Ok(new { message = "تم حذف سجل الحضور بنجاح." });
        }
    }
}

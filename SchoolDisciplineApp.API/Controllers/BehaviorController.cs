using Microsoft.AspNetCore.Mvc;
using SchoolDisciplineApp.Application.DTOs;
using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.API.Controllers
{
    /// <summary>
    /// Manages student behavior records (CRUD operations).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BehaviorController : ControllerBase
    {
        private readonly IBehaviorService _behaviorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorController"/> class.
        /// </summary>
        /// <param name="behaviorService">Service responsible for managing behavior records.</param>
        public BehaviorController ( IBehaviorService behaviorService )
        {
            _behaviorService = behaviorService;
        }

        /// <summary>
        /// Retrieves all behavior records.
        /// </summary>
        /// <param name="classId">Optional: Filter by class ID.</param>
        /// <param name="studentId">Optional: Filter by student ID.</param>
        /// <returns>List of all behavior records matching filters.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll ( [FromQuery] int? classId = null, [FromQuery] int? studentId = null )
        {
            var records = await _behaviorService.GetAllAsync(classId, studentId);
            var dtos = records.Select(x => new BehaviorRecordDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                BehaviorType = x.BehaviorType,
                Description = x.Description,
                Date = x.Date
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Retrieves a specific behavior record by its unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the behavior record.</param>
        /// <returns>Behavior record details.</returns>
        /// <response code="200">Returns the behavior record successfully.</response>
        /// <response code="404">If no record found with the given ID.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById ( int id )
        {
            var record = await _behaviorService.GetByIdAsync(id);
            if (record == null)
                return NotFound(new { message = $"لم يتم العثور على سجل السلوك بالمعرّف {id}." });

            var dto = new BehaviorRecordDto
            {
                Id = record.Id,
                StudentId = record.StudentId,
                BehaviorType = record.BehaviorType,
                Description = record.Description,
                Date = record.Date
            };

            return Ok(dto);
        }

        /// <summary>
        /// Retrieves all behavior records for a specific student.
        /// </summary>
        /// <param name="studentId">The unique ID of the student.</param>
        /// <returns>List of behavior records for the student.</returns>
        /// <response code="200">Returns records successfully.</response>
        /// <response code="404">If no records found for the student.</response>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId ( int studentId )
        {
            var records = await _behaviorService.GetByStudentIdAsync(studentId);
            if (records == null || !records.Any())
                return NotFound(new { message = $"لا توجد سجلات سلوك للطالب بالمعرّف {studentId}." });

            var dtos = records.Select(x => new BehaviorRecordDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                BehaviorType = x.BehaviorType,
                Description = x.Description,
                Date = x.Date
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Adds a new behavior record.
        /// </summary>
        /// <param name="dto">The behavior record DTO object to add.</param>
        /// <returns>The created behavior record.</returns>
        /// <response code="201">Behavior record added successfully.</response>
        /// <response code="400">If the input data is invalid.</response>
        [HttpPost]
        public async Task<IActionResult> Add ( [FromBody] BehaviorRecordDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "البيانات المدخلة غير صحيحة. يرجى التحقق من القيم المُرسلة." });

            var record = new BehaviorRecord
            {
                StudentId = dto.StudentId,
                BehaviorType = dto.BehaviorType,
                Description = dto.Description,
                Date = dto.Date
            };

            await _behaviorService.AddAsync(record);

            dto.Id = record.Id; // Set the generated Id back to DTO

            return CreatedAtAction(nameof(GetById), new { id = record.Id }, new
            {
                message = "تمت إضافة سجل السلوك بنجاح.",
                record = dto
            });
        }

        /// <summary>
        /// Updates an existing behavior record.
        /// </summary>
        /// <param name="id">The unique ID of the record to update.</param>
        /// <param name="dto">The updated behavior record DTO data.</param>
        /// <returns>Update result.</returns>
        /// <response code="200">Behavior record updated successfully.</response>
        /// <response code="400">If the input data is invalid.</response>
        /// <response code="404">If no record is found with the given ID.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update ( int id, [FromBody] BehaviorRecordDto dto )
        {
            if (id != dto.Id)
                return BadRequest(new { message = "معرّف سجل السلوك المرسل لا يتطابق مع المعرّف المطلوب تعديله." });

            var existingRecord = await _behaviorService.GetByIdAsync(id);
            if (existingRecord == null)
                return NotFound(new { message = $"لم يتم العثور على سجل السلوك بالمعرّف {id}." });

            existingRecord.StudentId = dto.StudentId;
            existingRecord.BehaviorType = dto.BehaviorType;
            existingRecord.Description = dto.Description;
            existingRecord.Date = dto.Date;

            await _behaviorService.UpdateAsync(existingRecord);

            return Ok(new { message = "تم تحديث سجل السلوك بنجاح." });
        }

        /// <summary>
        /// Deletes a behavior record by its ID.
        /// </summary>
        /// <param name="id">The unique ID of the record to delete.</param>
        /// <returns>Delete result.</returns>
        /// <response code="200">Behavior record deleted successfully.</response>
        /// <response code="404">If no record is found with the given ID.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var existingRecord = await _behaviorService.GetByIdAsync(id);
            if (existingRecord == null)
                return NotFound(new { message = $"لم يتم العثور على سجل السلوك بالمعرّف {id}." });

            await _behaviorService.DeleteAsync(id);

            return Ok(new { message = "تم حذف سجل السلوك بنجاح." });
        }
    }
}

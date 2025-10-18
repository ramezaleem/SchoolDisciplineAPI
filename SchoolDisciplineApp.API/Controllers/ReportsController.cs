using Microsoft.AspNetCore.Mvc;
using SchoolDisciplineApp.Application.DTOs;
using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.API.Controllers
{
    /// <summary>
    /// Manages reports in the system, including listing, retrieving, adding, and exporting reports.
    /// Aggregates data from Attendance, Behaviors, and Students.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportsController"/> class.
        /// </summary>
        /// <param name="reportService">Service responsible for managing reports.</param>
        public ReportsController ( IReportService reportService )
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Retrieves all reports in the system.
        /// </summary>
        /// <returns>List of all reports.</returns>
        /// <response code="200">Returns the list of reports.</response>
        /// <response code="404">If no reports exist.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll ()
        {
            try
            {
                var reports = await _reportService.GetAllAsync();
                if (reports == null || !reports.Any())
                    return NotFound(new { message = "لا توجد تقارير." });

                var dtos = reports.Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportType = r.ReportType,
                    GeneratedAt = r.GeneratedAt,
                    ReportData = r.ReportData
                });

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء جلب التقارير.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific report by its unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the report.</param>
        /// <returns>The report with the specified ID.</returns>
        /// <response code="200">Returns the requested report.</response>
        /// <response code="404">If the report with the given ID does not exist.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReportDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById ( int id )
        {
            try
            {
                var report = await _reportService.GetByIdAsync(id);
                if (report == null)
                    return NotFound(new { message = $"لم يتم العثور على التقرير بالمعرّف {id}." });

                var dto = new ReportDto
                {
                    Id = report.Id,
                    ReportType = report.ReportType,
                    GeneratedAt = report.GeneratedAt,
                    ReportData = report.ReportData
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء جلب التقرير.", error = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new report to the system.
        /// </summary>
        /// <param name="dto">The report DTO object to add.</param>
        /// <returns>The created report DTO.</returns>
        /// <response code="201">Report successfully created.</response>
        /// <response code="400">If the report data is invalid.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ReportDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Add ( [FromBody] ReportDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "البيانات المدخلة غير صحيحة." });

            try
            {
                var report = new Report
                {
                    ReportType = dto.ReportType,
                    GeneratedAt = dto.GeneratedAt,
                    ReportData = dto.ReportData
                };

                await _reportService.AddAsync(report);

                dto.Id = report.Id; // تعيين الرقم بعد الإضافة

                return CreatedAtAction(nameof(GetById), new { id = report.Id }, new
                {
                    message = "تمت إضافة التقرير بنجاح.",
                    report = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء إضافة التقرير.", error = ex.Message });
            }
        }

        /// <summary>
        /// Exports all reports in JSON format.
        /// </summary>
        /// <returns>A JSON list of all reports.</returns>
        /// <response code="200">Returns the exported reports.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPost("export")]
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Export ()
        {
            try
            {
                var reports = await _reportService.ExportReportsAsync();

                var dtos = reports.Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportType = r.ReportType,
                    GeneratedAt = r.GeneratedAt,
                    ReportData = r.ReportData
                });

                return Ok(new
                {
                    message = "تم تصدير التقارير بنجاح.",
                    data = dtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء تصدير التقارير.", error = ex.Message });
            }
        }
    }
}

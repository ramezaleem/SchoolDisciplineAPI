using OfficeOpenXml;
using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Infrastructure.Repositories;
namespace SchoolDisciplineApp.Application.Services.Implementations;

public class ExcelImportService : IExcelImportService
{
    private readonly IStudentRepository _studentRepository;

    public ExcelImportService ( IStudentRepository studentRepository )
    {
        _studentRepository = studentRepository;
    }

    public async Task ProcessExcelFileAsync ( Stream fileStream )
    {
        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];
        int rowCount = worksheet.Dimension.Rows;

        for (int row = 2 ; row <= rowCount ; row++)
        {
            var student = new Student
            {
                Name = worksheet.Cells[row, 1].Text,
                ClassId = int.Parse(worksheet.Cells[row, 2].Text),
                // أكمل باقي الحقول بناءً على تنسيق الملف
            };

            await _studentRepository.AddAsync(student);
        }

        await _studentRepository.SaveChangesAsync();
    }

}
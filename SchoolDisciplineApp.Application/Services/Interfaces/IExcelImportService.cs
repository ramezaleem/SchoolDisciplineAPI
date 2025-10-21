namespace SchoolDisciplineApp.Application.Services.Interfaces;
public interface IExcelImportService
{
    Task ProcessExcelFileAsync ( Stream fileStream );
}
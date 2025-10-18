using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.Application.Services.Interfaces;
public interface IReportService
{
    Task<IEnumerable<Report>> GetAllAsync ();
    Task<Report?> GetByIdAsync ( int id );
    Task AddAsync ( Report report );
    Task<IEnumerable<Report>> ExportReportsAsync ();
}
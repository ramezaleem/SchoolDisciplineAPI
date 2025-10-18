using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public interface IReportRepository
    {
        IQueryable<Report> GetAllQueryable ();
        Task<IEnumerable<Report>> GetAllAsync ();
        Task<Report?> GetByIdAsync ( int id );
        Task AddAsync ( Report report );
        Task<IEnumerable<Report>> ExportReportsAsync ();
    }
}

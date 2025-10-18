using Microsoft.EntityFrameworkCore;
using SchoolDisciplineApp.Domain.Entities;
using SchoolDisciplineApp.Infrastructure.Data;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly SchoolDisciplineDbContext _dbContext;

        public ReportRepository ( SchoolDisciplineDbContext dbContext )
        {
            _dbContext = dbContext;
        }

        public IQueryable<Report> GetAllQueryable ()
        {
            return _dbContext.Reports.AsQueryable();
        }

        public async Task<IEnumerable<Report>> GetAllAsync ()
        {
            return await _dbContext.Reports.ToListAsync();
        }

        public async Task<Report?> GetByIdAsync ( int id )
        {
            return await _dbContext.Reports.FindAsync(id);
        }

        public async Task AddAsync ( Report report )
        {
            await _dbContext.Reports.AddAsync(report);
            await _dbContext.SaveChangesAsync();
        }

        // مثال على Export: ترجع كل التقارير بصيغة JSON أو string
        public async Task<IEnumerable<Report>> ExportReportsAsync ()
        {
            return await _dbContext.Reports.ToListAsync();
        }
    }
}

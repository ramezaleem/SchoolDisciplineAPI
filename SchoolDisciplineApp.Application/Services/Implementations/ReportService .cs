using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Domain.Entities;
using SchoolDisciplineApp.Infrastructure.Repositories;

namespace SchoolDisciplineApp.Application.Services.Implementations;
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService ( IReportRepository reportRepository )
    {
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<Report>> GetAllAsync ()
    {
        return await _reportRepository.GetAllAsync();
    }

    public async Task<Report?> GetByIdAsync ( int id )
    {
        return await _reportRepository.GetByIdAsync(id);
    }

    public async Task AddAsync ( Report report )
    {
        await _reportRepository.AddAsync(report);
    }

    public async Task<IEnumerable<Report>> ExportReportsAsync ()
    {
        return await _reportRepository.ExportReportsAsync();
    }
}


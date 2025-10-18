using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.Application.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceRecord>> GetAllAsync ( int? classId = null, int? month = null );
        Task<AttendanceRecord?> GetByIdAsync ( int id );
        Task<IEnumerable<AttendanceRecord>> GetByStudentIdAsync ( int studentId );
        Task<IEnumerable<AttendanceRecord>> GetForClassByDateAsync ( int classId, DateTime date );
        Task AddAsync ( AttendanceRecord record );
        Task UpdateAsync ( AttendanceRecord record );
        Task DeleteAsync ( int id );
    }
}

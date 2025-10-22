using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public interface IAttendanceRepository
    {
        IQueryable<AttendanceRecord> GetAllQueryable ();

        Task<IEnumerable<AttendanceRecord>> GetAllAsync ();
        Task<AttendanceRecord?> GetByIdAsync ( int id );
        Task<IEnumerable<AttendanceRecord>> GetByStudentIdAsync ( int studentId );
        Task<IEnumerable<AttendanceRecord>> GetForClassByDateAsync ( int classId, DateTime date );
        Task AddAsync ( AttendanceRecord record );
        Task UpdateAsync ( AttendanceRecord record );
        Task DeleteAsync ( int id );
        Task<Dictionary<string, int>> GetAbsenceStatsByClassForMonthAsync ( int classId, int year, int month );
        Task<IEnumerable<AttendanceRecord>> GetByStudentAbsencesAsync ( int studentId, bool? isAbsent = null, bool? isExcused = null );
    }
}

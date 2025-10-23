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
        Task<Dictionary<string, int>> GetAbsenceStatsByClassForMonthAsync ( int classId, int year, int month );
        Task<IEnumerable<AttendanceRecord>> GetByStudentAbsencesAsync ( int studentId, bool? isAbsent = null, bool? isExcused = null );
        Task<List<Student>> GetStudentsWithNoAbsenceInMonthAsync ( int classId, int year, int month );
        Task<int> GetAbsenceDaysCountAsync ( int studentId, DateTime startDate, DateTime endDate, bool? isExcused = null );
    }
}

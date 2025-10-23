using Microsoft.EntityFrameworkCore;
using SchoolDisciplineApp.Domain.Entities;
using SchoolDisciplineApp.Infrastructure.Data;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly SchoolDisciplineDbContext _dbContext;

        public AttendanceRepository ( SchoolDisciplineDbContext dbContext )
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AttendanceRecord>> GetAllAsync ()
        {
            return await _dbContext.AttendanceRecords
                .Include(a => a.Student)
                .Include(a => a.Class)
                .ToListAsync();
        }

        public async Task<AttendanceRecord?> GetByIdAsync ( int id )
        {
            return await _dbContext.AttendanceRecords
                .Include(a => a.Student)
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AttendanceRecord>> GetByStudentIdAsync ( int studentId )
        {
            return await _dbContext.AttendanceRecords
                .Where(r => r.StudentId == studentId)
                .Include(r => r.Student)
                .Include(r => r.Class)
                .ToListAsync();
        }

        public async Task<IEnumerable<AttendanceRecord>> GetForClassByDateAsync ( int classId, DateTime date )
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _dbContext.AttendanceRecords
                .Where(a => a.ClassId == classId && a.Date >= start && a.Date < end)
                .Include(a => a.Student)
                .Include(a => a.Class)
                .ToListAsync();
        }

        public IQueryable<AttendanceRecord> GetAllQueryable ()
        {
            return _dbContext.AttendanceRecords
                .Include(a => a.Student)
                .Include(a => a.Class)
                .AsQueryable();
        }

        public async Task AddAsync ( AttendanceRecord record )
        {
            await _dbContext.AttendanceRecords.AddAsync(record);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync ( AttendanceRecord record )
        {
            _dbContext.AttendanceRecords.Update(record);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync ( int id )
        {
            var record = await _dbContext.AttendanceRecords.FindAsync(id);
            if (record != null)
            {
                _dbContext.AttendanceRecords.Remove(record);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AttendanceRecord>> GetByStudentAbsencesAsync ( int studentId, bool? isAbsent = null, bool? isExcused = null )
        {
            var query = _dbContext.AttendanceRecords
                .Where(r => r.StudentId == studentId)
                .Include(r => r.Student)
                .Include(r => r.Class)
                .AsQueryable();

            if (isAbsent.HasValue)
                query = query.Where(r => r.IsAbsent == isAbsent.Value);
            if (isExcused.HasValue)
                query = query.Where(r => r.IsExcused == isExcused.Value);

            return await query.ToListAsync();
        }
        public async Task<Dictionary<string, int>> GetAbsenceStatsByClassForMonthAsync ( int classId, int year, int month )
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var query = _dbContext.AttendanceRecords
                .Where(a => a.ClassId == classId &&
                            a.IsAbsent == true &&
                            a.Date >= startDate &&
                            a.Date <= endDate)
                .GroupBy(a => a.StudentId)
                .Select(g => new { StudentId = g.Key, AbsenceCount = g.Count() });

            var grouped = await query.ToListAsync();

            int count10 = grouped.Count(x => x.AbsenceCount >= 10);
            int count5 = grouped.Count(x => x.AbsenceCount >= 5);
            int count3 = grouped.Count(x => x.AbsenceCount >= 3);

            return new Dictionary<string, int>
            {
                { "10DaysOrMore", count10 },
                { "5DaysOrMore", count5 },
                { "3DaysOrMore", count3 }
            };
        }

        public async Task<List<Student>> GetStudentsWithNoAbsenceInMonthAsync ( int classId, int year, int month )
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var studentsInClass = await _dbContext.Students
                .Where(s => s.ClassId == classId)
                .ToListAsync();

            var absentStudentIds = await _dbContext.AttendanceRecords
                .Where(a => a.ClassId == classId &&
                            a.IsAbsent == true &&
                            a.Date >= startDate &&
                            a.Date <= endDate)
                .Select(a => a.StudentId)
                .Distinct()
                .ToListAsync();

            return studentsInClass
                .Where(s => !absentStudentIds.Contains(s.Id))
                .ToList();
        }


    }
}

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



    }
}

using Microsoft.EntityFrameworkCore;
using SchoolDisciplineApp.Domain.Entities;
using SchoolDisciplineApp.Infrastructure.Data;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public class BehaviorRepository : IBehaviorRepository
    {
        private readonly SchoolDisciplineDbContext _context;

        public BehaviorRepository ( SchoolDisciplineDbContext context )
        {
            _context = context;
        }

        public IQueryable<BehaviorRecord> GetAllQueryable ()
        {
            return _context.BehaviorRecords
                .Include(b => b.Student)
                .AsQueryable();
        }

        public async Task<IEnumerable<BehaviorRecord>> GetAllAsync ()
        {
            return await _context.BehaviorRecords
                .Include(b => b.Student)
                .ToListAsync();
        }

        public async Task<BehaviorRecord?> GetByIdAsync ( int id )
        {
            return await _context.BehaviorRecords
                .Include(b => b.Student)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BehaviorRecord>> GetByStudentIdAsync ( int studentId )
        {
            return await _context.BehaviorRecords
                .Where(b => b.StudentId == studentId)
                .Include(b => b.Student)
                .ToListAsync();
        }

        public async Task AddAsync ( BehaviorRecord record )
        {
            await _context.BehaviorRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync ( BehaviorRecord record )
        {
            _context.BehaviorRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync ( int id )
        {
            var record = await _context.BehaviorRecords.FindAsync(id);
            if (record != null)
            {
                _context.BehaviorRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}

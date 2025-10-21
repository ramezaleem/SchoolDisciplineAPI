using Microsoft.EntityFrameworkCore;
using SchoolDisciplineApp.Infrastructure.Data;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SchoolDisciplineDbContext _context;

        public StudentRepository ( SchoolDisciplineDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync ()
        {
            return await _context.Students
                .Include(s => s.Class)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync ( int id )
        {
            return await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Student>> GetByClassIdAsync ( int classId )
        {
            return await _context.Students
                .Where(s => s.ClassId == classId)
                .ToListAsync();
        }

        public async Task AddAsync ( Student student )
        {
            await _context.Students.AddAsync(student);
        }

        public async Task SaveChangesAsync ()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync ( Student student )
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync ( int id )
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}

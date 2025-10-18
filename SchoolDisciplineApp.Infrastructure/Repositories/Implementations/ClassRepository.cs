using Microsoft.EntityFrameworkCore;
using SchoolDisciplineApp.Domain.Entities;
using SchoolDisciplineApp.Infrastructure.Data;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly SchoolDisciplineDbContext _context;

        public ClassRepository ( SchoolDisciplineDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<SchoolClass>> GetAllAsync ()
        {
            return await _context.Classes
                .Include(c => c.Students)
                .ToListAsync();
        }

        public async Task<SchoolClass?> GetByIdAsync ( int id )
        {
            return await _context.Classes
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync ( SchoolClass classEntity )
        {
            await _context.Classes.AddAsync(classEntity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync ( SchoolClass classEntity )
        {
            _context.Classes.Update(classEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync ( int id )
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity != null)
            {
                _context.Classes.Remove(classEntity);
                await _context.SaveChangesAsync();
            }
        }
    }
}

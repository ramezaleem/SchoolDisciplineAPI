using Microsoft.EntityFrameworkCore;
using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Domain.Entities;
using SchoolDisciplineApp.Infrastructure.Repositories;

namespace SchoolDisciplineApp.Application.Services.Implementations
{
    public class BehaviorService : IBehaviorService
    {
        private readonly IBehaviorRepository _behaviorRepository;

        public BehaviorService ( IBehaviorRepository behaviorRepository )
        {
            _behaviorRepository = behaviorRepository;
        }

        // Retrieves all behavior records, optionally filtered by classId and/or studentId
        public async Task<IEnumerable<BehaviorRecord>> GetAllAsync ( int? classId = null, int? studentId = null )
        {
            var query = _behaviorRepository.GetAllQueryable();

            if (classId.HasValue)
                query = query.Where(b => b.Student.ClassId == classId.Value);

            if (studentId.HasValue)
                query = query.Where(b => b.StudentId == studentId.Value);

            return await query.ToListAsync();
        }

        public async Task<BehaviorRecord?> GetByIdAsync ( int id )
        {
            return await _behaviorRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BehaviorRecord>> GetByStudentIdAsync ( int studentId )
        {
            return await _behaviorRepository.GetByStudentIdAsync(studentId);
        }

        public async Task AddAsync ( BehaviorRecord record )
        {
            await _behaviorRepository.AddAsync(record);
        }

        public async Task UpdateAsync ( BehaviorRecord record )
        {
            await _behaviorRepository.UpdateAsync(record);
        }

        public async Task DeleteAsync ( int id )
        {
            await _behaviorRepository.DeleteAsync(id);
        }
    }
}

using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.Application.Services.Interfaces
{
    public interface IBehaviorService
    {
        Task<IEnumerable<BehaviorRecord>> GetAllAsync ( int? classId = null, int? studentId = null );
        Task<BehaviorRecord?> GetByIdAsync ( int id );
        Task<IEnumerable<BehaviorRecord>> GetByStudentIdAsync ( int studentId );
        Task AddAsync ( BehaviorRecord record );
        Task UpdateAsync ( BehaviorRecord record );
        Task DeleteAsync ( int id );
    }
}

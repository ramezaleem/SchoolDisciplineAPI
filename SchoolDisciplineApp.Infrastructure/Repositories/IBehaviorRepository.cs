using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public interface IBehaviorRepository
    {
        IQueryable<BehaviorRecord> GetAllQueryable ();

        Task<IEnumerable<BehaviorRecord>> GetAllAsync ();
        Task<BehaviorRecord?> GetByIdAsync ( int id );
        Task<IEnumerable<BehaviorRecord>> GetByStudentIdAsync ( int studentId );
        Task AddAsync ( BehaviorRecord record );
        Task UpdateAsync ( BehaviorRecord record );
        Task DeleteAsync ( int id );
    }
}

using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public interface IClassRepository
    {
        Task<IEnumerable<SchoolClass>> GetAllAsync ();
        Task<SchoolClass?> GetByIdAsync ( int id );
        Task AddAsync ( SchoolClass classEntity );
        Task UpdateAsync ( SchoolClass classEntity );
        Task DeleteAsync ( int id );
    }
}

namespace SchoolDisciplineApp.Infrastructure.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync ();
        Task<Student?> GetByIdAsync ( int id );
        Task<IEnumerable<Student>> GetByClassIdAsync ( int classId );
        Task AddAsync ( Student student );
        Task UpdateAsync ( Student student );
        Task DeleteAsync ( int id );
        Task SaveChangesAsync ();
    }
}

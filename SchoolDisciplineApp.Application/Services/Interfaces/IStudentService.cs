namespace SchoolDisciplineApp.Application.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllAsync ();
        Task<Student?> GetByIdAsync ( int id );
        Task<IEnumerable<Student>> GetByClassIdAsync ( int classId );

        Task<Student> AddAsync ( Student student );
        Task UpdateAsync ( Student student );
        Task DeleteAsync ( int id );
    }
}

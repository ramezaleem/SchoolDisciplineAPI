namespace SchoolDisciplineApp.Application.Services.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<SchoolClass>> GetAllAsync ();
        Task<SchoolClass?> GetByIdAsync ( int id );
        Task<IEnumerable<Student>> GetStudentsByClassIdAsync ( int classId );
        Task AddAsync ( SchoolClass schoolClass );
        Task UpdateAsync ( SchoolClass schoolClass );
        Task DeleteAsync ( int id );
    }
}

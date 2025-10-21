using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Infrastructure.Repositories;

namespace SchoolDisciplineApp.Application.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService ( IStudentRepository studentRepository )
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<Student>> GetAllAsync ()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task<Student?> GetByIdAsync ( int id )
        {
            return await _studentRepository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<Student>> GetByClassIdAsync ( int classId )
        {
            return await _studentRepository.GetByClassIdAsync(classId);
        }

        public async Task<Student> AddAsync ( Student student )
        {
            return await _studentRepository.AddAsync(student);
        }

        public async Task UpdateAsync ( Student student )
        {
            await _studentRepository.UpdateAsync(student);
        }

        public async Task DeleteAsync ( int id )
        {
            await _studentRepository.DeleteAsync(id);
        }
    }
}

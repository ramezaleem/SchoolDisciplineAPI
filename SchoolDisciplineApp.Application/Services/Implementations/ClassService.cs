using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Domain.Entities;
using SchoolDisciplineApp.Infrastructure.Repositories;

namespace SchoolDisciplineApp.Application.Services.Implementations
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;

        public ClassService ( IClassRepository classRepository )
        {
            _classRepository = classRepository;
        }

        public async Task<IEnumerable<SchoolClass>> GetAllAsync ()
        {
            return await _classRepository.GetAllAsync();
        }

        public async Task<SchoolClass?> GetByIdAsync ( int id )
        {
            return await _classRepository.GetByIdAsync(id);
        }

        public async Task AddAsync ( SchoolClass schoolClass )
        {
            await _classRepository.AddAsync(schoolClass);
        }

        public async Task UpdateAsync ( SchoolClass schoolClass )
        {
            await _classRepository.UpdateAsync(schoolClass);
        }

        public async Task DeleteAsync ( int id )
        {
            await _classRepository.DeleteAsync(id);
        }
    }
}

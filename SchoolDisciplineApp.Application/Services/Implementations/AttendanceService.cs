using Microsoft.EntityFrameworkCore;
using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Domain.Entities;
using SchoolDisciplineApp.Infrastructure.Repositories;

namespace SchoolDisciplineApp.Application.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService ( IAttendanceRepository attendanceRepository )
        {
            _attendanceRepository = attendanceRepository;
        }

        /// <summary>
        /// Retrieves all attendance records, optionally filtered by class and month.
        /// </summary>
        /// <param name="classId">Optional class ID to filter records.</param>
        /// <param name="month">Optional month (1-12) to filter records.</param>
        public async Task<IEnumerable<AttendanceRecord>> GetAllAsync ( int? classId = null, int? month = null )
        {
            var query = _attendanceRepository.GetAllQueryable();

            if (classId.HasValue)
                query = query.Where(r => r.ClassId == classId.Value);

            if (month.HasValue)
                query = query.Where(r => r.Date.Month == month.Value);

            return await query.ToListAsync();
        }

        public async Task<AttendanceRecord?> GetByIdAsync ( int id )
        {
            return await _attendanceRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AttendanceRecord>> GetByStudentIdAsync ( int studentId )
        {
            return await _attendanceRepository.GetByStudentIdAsync(studentId);
        }

        public async Task AddAsync ( AttendanceRecord record )
        {
            await _attendanceRepository.AddAsync(record);
        }

        public async Task UpdateAsync ( AttendanceRecord record )
        {
            await _attendanceRepository.UpdateAsync(record);
        }

        public async Task DeleteAsync ( int id )
        {
            await _attendanceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AttendanceRecord>> GetForClassByDateAsync ( int classId, DateTime date )
        {
            return await _attendanceRepository.GetForClassByDateAsync(classId, date);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using SchoolDisciplineApp.Domain.Entities;

namespace SchoolDisciplineApp.Infrastructure.Data
{
    public class SchoolDisciplineDbContext : DbContext
    {
        public SchoolDisciplineDbContext ( DbContextOptions<SchoolDisciplineDbContext> options )
            : base(options)
        {
        }

        public DbSet<SchoolClass> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<BehaviorRecord> BehaviorRecords { get; set; }
        public DbSet<Report> Reports { get; set; }


        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.Student)
                .WithMany(s => s.AttendanceRecords)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.Class)
                .WithMany()
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Cascade);
        }



    }
}

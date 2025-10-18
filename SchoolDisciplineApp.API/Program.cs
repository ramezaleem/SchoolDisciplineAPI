using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SchoolDisciplineApp.Application.Services.Implementations;
using SchoolDisciplineApp.Application.Services.Interfaces;
using SchoolDisciplineApp.Infrastructure.Data;
using SchoolDisciplineApp.Infrastructure.Repositories;
using System.Reflection;

namespace SchoolDisciplineApp.API
{
    public class Program
    {
        public static void Main ( string[] args )
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<SchoolDisciplineDbContext>(options =>
                 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            builder.Services.AddScoped<IBehaviorRepository, BehaviorRepository>();
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();

            // Services
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<IBehaviorService, BehaviorService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<IReportService, ReportService>();

            builder.Services.AddControllers();

            // CORS Policy: íÓãÍ áÃí Origin ÈØáÈ ÇáÜ API (Íá ÔÇãá áãÔÇßá CORS/frontend)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", corsBuilder =>
                {
                    corsBuilder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Swagger & XML Comments
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "School Discipline API",
                    Version = "v1",
                    Description = "API documentation for School Discipline system"
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            var app = builder.Build();

            // ÇáÊæÌíå äÍæ HTTPS ÅÐÇ ÊæÝøÑ
            app.UseHttpsRedirection();

            // ÊÝÚíá CORS ÞÈá ßá ÔíÁ (ÖÑæÑí áÍá ÇáãÔÇßá)
            app.UseCors("AllowAll");

            // Middleware Authorization (Åä æÌÏ ÍãÇíÉ ÇáåæíÉ)
            app.UseAuthorization();

            // ÊÝÚíá Swagger ææÇÌåÉ ÇáÊæËíÞ
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Discipline API v1");
                c.RoutePrefix = "swagger";
                c.ConfigObject.AdditionalItems["syntaxHighlight"] = false;
                c.ConfigObject.AdditionalItems["tryItOutEnabled"] = true;
            });

            // ÅÚÇÏÉ ÊæÌíå ÇáÕÝÍÉ ÇáÑÆíÓíÉ Åáì Swagger
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.MapControllers();

            app.Run();
        }
    }
}

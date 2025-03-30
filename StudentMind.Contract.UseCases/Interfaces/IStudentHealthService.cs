using StudentMind.Core.Entity;
using StudentMind.Services.DTO;

namespace StudentMind.Services.Interfaces
{
    public interface IStudentHealthService
    {
        Task<StudentHealth> CreateStudentHealth(StudentHealthDTO studentHealthDto);
        Task<StudentHealth> UpdateStudentHealth(string id, StudentHealthDTO studentHealthDto);
        Task DeleteStudentHealth(string id);
        Task<StudentHealth> GetStudentHealthById(string id);
        Task<List<StudentHealth>> GetStudentHealths();
    }
}

using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.Services
{
    public class StudentHealthService : IStudentHealthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentHealthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentHealth> CreateStudentHealth(StudentHealthDTO studentHealthDto)
        {
            var studentHealthRepo = _unitOfWork.GetRepository<StudentHealth>();
            var studentHealth = new StudentHealth
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                StudentId = studentHealthDto.StudentId,
                SurveyId = studentHealthDto.SurveyId,
                Score = studentHealthDto.Score,
                Category = studentHealthDto.Category,
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now
            };
            await studentHealthRepo.InsertAsync(studentHealth);
            await _unitOfWork.SaveAsync();
            return studentHealth;
        }

        public async Task DeleteStudentHealth(string id)
        {
            var studentHealthRepo = _unitOfWork.GetRepository<StudentHealth>();
            StudentHealth studentHealth = studentHealthRepo.GetById(id);
            await studentHealthRepo.DeleteAsync(studentHealth);
            await _unitOfWork.SaveAsync();
        }

        public async Task<StudentHealth> GetStudentHealthById(string id)
        {
            var studentHealthRepo = _unitOfWork.GetRepository<StudentHealth>();
            return await studentHealthRepo.Entities.Include(sh => sh.Student).Include(sh => sh.Survey).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StudentHealth> GetStudentHealthByUserIdSurveyId(string userId, string surveyId)
        {
            var studentHealthRepo = _unitOfWork.GetRepository<StudentHealth>();
            return await studentHealthRepo.Entities.Include(sh => sh.Student).Include(sh => sh.Survey).FirstOrDefaultAsync(s => s.StudentId == userId && s.SurveyId == surveyId);
        }

        public async Task<List<StudentHealth>> GetStudentHealths()
        {
            var StudentHealthRepo = _unitOfWork.GetRepository<StudentHealth>();
            return StudentHealthRepo.Entities.Include(sh => sh.Student).Include(sh => sh.Survey).ToList();
        }

        public async Task<StudentHealth> UpdateStudentHealth(string id, StudentHealthDTO studentHealthDto)
        {
            var studentHealthRepo = _unitOfWork.GetRepository<StudentHealth>();
            var studentHealth = await studentHealthRepo.GetByIdAsync(id);
            studentHealth.StudentId = studentHealthDto.StudentId;
            studentHealth.SurveyId = studentHealthDto.SurveyId;
            studentHealth.Score = studentHealthDto.Score;
            studentHealth.Category = studentHealthDto.Category;
            studentHealth.LastUpdatedTime = DateTime.Now;
            await studentHealthRepo.UpdateAsync(studentHealth);
            await _unitOfWork.SaveAsync();
            return studentHealth;
        }
    }
}

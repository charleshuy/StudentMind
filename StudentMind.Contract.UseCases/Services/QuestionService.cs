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
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Question> CreateQuestion(QuestionDTO questionDto)
        {
            var questionRepo = _unitOfWork.GetRepository<Question>();
            var question = new Question
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                Content = questionDto.Content,
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now
            };
            await questionRepo.InsertAsync(question);
            await _unitOfWork.SaveAsync();
            return question;
        }

        public async Task DeleteQuestion(string id)
        {
            var questionRepo = _unitOfWork.GetRepository<Question>();
            Question question = questionRepo.GetById(id);
            await questionRepo.DeleteAsync(question);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Question> GetQuestionById(string id)
        {
            var questionRepo = _unitOfWork.GetRepository<Question>();
            return await questionRepo.Entities.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Question>> GetQuestions()
        {
            var questionRepo = _unitOfWork.GetRepository<Question>();
            return questionRepo.Entities.ToList();
        }

        public async Task<Question> UpdateQuestion(string id, QuestionDTO questionDto)
        {
            var questionRepo = _unitOfWork.GetRepository<Question>();
            var question = await questionRepo.GetByIdAsync(id);
            question.Content = questionDto.Content;
            question.LastUpdatedTime = DateTime.Now;
            await questionRepo.UpdateAsync(question);
            await _unitOfWork.SaveAsync();
            return question;
        }
    }
}

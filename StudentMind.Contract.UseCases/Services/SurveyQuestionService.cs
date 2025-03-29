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
    public class SurveyQuestionService : ISurveyQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SurveyQuestionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SurveyQuestion> CreateSurveyQuestion(SurveyQuestionDTO surveyQuestionDto)
        {
            var surveyQuestionRepo = _unitOfWork.GetRepository<SurveyQuestion>();
            var surveyQuestion = new SurveyQuestion
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                SurveyId = surveyQuestionDto.SurveyId,
                QuestionId = surveyQuestionDto.QuestionId,
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now
            };
            await surveyQuestionRepo.InsertAsync(surveyQuestion);
            await _unitOfWork.SaveAsync();
            return surveyQuestion;
        }

        public async Task DeleteSurveyQuestion(string id)
        {
            var surveyQuestionRepo = _unitOfWork.GetRepository<SurveyQuestion>();
            SurveyQuestion surveyQuestion = surveyQuestionRepo.GetById(id);
            await surveyQuestionRepo.DeleteAsync(surveyQuestion);
            await _unitOfWork.SaveAsync();
        }

        public async Task<SurveyQuestion> GetSurveyQuestionById(string id)
        {
            var surveyQuestionRepo = _unitOfWork.GetRepository<SurveyQuestion>();
            return await surveyQuestionRepo.Entities.Include(s => s.Survey).Include(s => s.Question).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SurveyQuestion>> GetSurveyQuestions()
        {
            var surveyQuestionRepo = _unitOfWork.GetRepository<SurveyQuestion>();
            return surveyQuestionRepo.Entities.Include(s => s.Survey).Include(s => s.Question).ToList();
        }

        public async Task<SurveyQuestion> UpdateSurveyQuestion(string id, SurveyQuestionDTO surveyQuestionDto)
        {
            var surveyQuestionRepo = _unitOfWork.GetRepository<SurveyQuestion>();
            var surveyQuestion = await surveyQuestionRepo.GetByIdAsync(id);
            surveyQuestion.SurveyId = surveyQuestionDto.SurveyId;
            surveyQuestion.QuestionId = surveyQuestionDto.QuestionId;
            surveyQuestion.LastUpdatedTime = DateTime.Now;
            await surveyQuestionRepo.UpdateAsync(surveyQuestion);
            await _unitOfWork.SaveAsync();
            return surveyQuestion;
        }
    }
}

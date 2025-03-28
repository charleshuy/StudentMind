using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Services.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SurveyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Survey> CreateSurvey(SurveyDTO surveyDto)
        {
            var surveyRepo = _unitOfWork.GetRepository<Survey>();
            var survey = new Survey
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                Name = surveyDto.Name,
                Description = surveyDto.Description,
                StartDate = surveyDto.StartDate,
                EndDate = surveyDto.EndDate,
                TotalParticipants = surveyDto.TotalParticipants,
                TypeId = surveyDto.TypeId,
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now
            };
            await surveyRepo.InsertAsync(survey);
            await _unitOfWork.SaveAsync();
            return survey;
        }

        public async Task<Survey> UpdateSurvey(string id, SurveyDTO surveyDto)
        {
            var surveyRepo = _unitOfWork.GetRepository<Survey>();
            var survey = await surveyRepo.GetByIdAsync(id);
            survey.Name = surveyDto.Name;
            survey.Description = surveyDto.Description;
            survey.StartDate = surveyDto.StartDate;
            survey.EndDate = surveyDto.EndDate;
            survey.TotalParticipants = surveyDto.TotalParticipants;
            survey.TypeId = surveyDto.TypeId;
            survey.LastUpdatedTime = DateTime.Now;
            await surveyRepo.UpdateAsync(survey);
            await _unitOfWork.SaveAsync();
            return survey;
        }

        public async Task DeleteSurvey(string id)
        {
            var surveyRepo = _unitOfWork.GetRepository<Survey>();
            Survey survey = surveyRepo.GetById(id);
            await surveyRepo.DeleteAsync(survey);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Survey> GetSurveyById(string id)
        {
            var surveyRepo = _unitOfWork.GetRepository<Survey>();
            return await surveyRepo.Entities.Include(s => s.Type).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Survey>> GetSurveys()
        {
            var surveyRepo = _unitOfWork.GetRepository<Survey>();
            return surveyRepo.Entities.Include(s => s.Type).ToList();
        }
    }
}

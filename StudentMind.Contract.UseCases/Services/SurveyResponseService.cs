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
    public class SurveyResponseService : ISurveyResponseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SurveyResponseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<SurveyResponse> CreateSurveyResponse(SurveyResponseDTO surveyResponseDto)
        {
            var surveyResponseRepo = _unitOfWork.GetRepository<SurveyResponse>();
            var surveyResponse = new SurveyResponse
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                SurveyId = surveyResponseDto.SurveyId,
                UserId = surveyResponseDto.UserId,
                QuestionId = surveyResponseDto.QuestionId,
                ChoiceId = surveyResponseDto.ChoiceId,
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now
            };
            await surveyResponseRepo.InsertAsync(surveyResponse);
            await _unitOfWork.SaveAsync();
            return surveyResponse;
        }

        public async Task DeleteSurveyResponse(string id)
        {
            var surveyResponseRepo = _unitOfWork.GetRepository<SurveyResponse>();
            SurveyResponse surveyResponse = surveyResponseRepo.GetById(id);
            await surveyResponseRepo.DeleteAsync(surveyResponse);
            await _unitOfWork.SaveAsync();
        }

        public async Task<SurveyResponse> GetSurveyResponseById(string id)
        {
            var surveyResponseRepo = _unitOfWork.GetRepository<SurveyResponse>();
            return await surveyResponseRepo.Entities.Include(sr => sr.User).Include(sr => sr.Survey).Include(sr => sr.Question).Include(sr => sr.Choice).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SurveyResponse>> GetSurveyResponses()
        {
            var surveyResponseRepo = _unitOfWork.GetRepository<SurveyResponse>();
            return surveyResponseRepo.Entities.Include(sr => sr.User).Include(sr => sr.Survey).Include(sr => sr.Question).Include(sr => sr.Choice).ToList();
        }

        public async Task<SurveyResponse> UpdateSurveyResponse(string id, SurveyResponseDTO surveyResponseDto)
        {
            var surveyResponseRepo = _unitOfWork.GetRepository<SurveyResponse>();
            var surveyResponse = await surveyResponseRepo.GetByIdAsync(id);
            surveyResponse.SurveyId = surveyResponseDto.SurveyId;
            surveyResponse.UserId = surveyResponseDto.UserId;
            surveyResponse.QuestionId = surveyResponseDto.QuestionId;
            surveyResponse.ChoiceId = surveyResponseDto.ChoiceId;
            surveyResponse.LastUpdatedTime = DateTime.Now;
            await surveyResponseRepo.UpdateAsync(surveyResponse);
            await _unitOfWork.SaveAsync();
            return surveyResponse;
        }

        public async Task<List<SurveyResponse>> GetSurveyResponsesByUserIdSurveyId(string userId, string surveyId)
        {
            var surveyResponseRepo = _unitOfWork.GetRepository<SurveyResponse>();

            var responses = await surveyResponseRepo.Entities
                .Where(sr => sr.UserId == userId && sr.SurveyId == surveyId)
                .ToListAsync();

            // Round CreatedTime to the nearest minute (ignoring seconds and milliseconds)
            var roundedResponses = responses
                .Select(sr => new
                {
                    SurveyResponse = sr,
                    RoundedTime = new DateTime(sr.CreatedTime.Year, sr.CreatedTime.Month, sr.CreatedTime.Day,
                                              sr.CreatedTime.Hour, sr.CreatedTime.Minute, 0) // Round to the minute
                })
                .GroupBy(sr => sr.RoundedTime) // Group by the rounded time
                .OrderByDescending(g => g.Key) // Order by the most recent rounded time
                .FirstOrDefault(); // Get the most recent group

            if (roundedResponses != null)
            {
                // Return the list of responses that belong to the most recent group
                return roundedResponses.Select(r => r.SurveyResponse).ToList();
            }

            // Return an empty list if no responses are found
            return new List<SurveyResponse>();
        }
    }
}

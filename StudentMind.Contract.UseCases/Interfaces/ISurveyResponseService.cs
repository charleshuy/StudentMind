using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.Interfaces
{
    public interface ISurveyResponseService
    {
        Task<SurveyResponse> CreateSurveyResponse(SurveyResponseDTO surveyResponseDto);
        Task<SurveyResponse> UpdateSurveyResponse(string id, SurveyResponseDTO surveyResponseDto);
        Task DeleteSurveyResponse(string id);
        Task<SurveyResponse> GetSurveyResponseById(string id);
        Task<List<SurveyResponse>> GetSurveyResponses();
    }
}

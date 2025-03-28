using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.Interfaces
{
    public interface ISurveyService
    {
        Task<Survey> CreateSurvey(SurveyDTO surveyDto);
        Task<Survey> UpdateSurvey(string id, SurveyDTO surveyDto);
        Task DeleteSurvey(string id);
        Task<Survey> GetSurveyById(string id);
        Task<List<Survey>> GetSurveys();
    }
}

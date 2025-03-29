using StudentMind.Core.Entity;
using StudentMind.Services.DTO;

namespace StudentMind.Services.Interfaces
{
    public interface ISurveyQuestionService
    {
        Task<SurveyQuestion> CreateSurveyQuestion(SurveyQuestionDTO surveyQuestionDto);
        //Task<SurveyQuestion> UpdateSurveyQuestion(string id, SurveyQuestionDTO surveyQuestionDto);
        Task DeleteSurveyQuestion(string id);
        Task<SurveyQuestion> GetSurveyQuestionById(string id);
        Task<List<SurveyQuestion>> GetSurveyQuestions();
    }
}

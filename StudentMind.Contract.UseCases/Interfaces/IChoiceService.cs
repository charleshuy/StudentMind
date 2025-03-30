using StudentMind.Core.Entity;
using StudentMind.Services.DTO;

namespace StudentMind.Services.Interfaces
{
    public interface IChoiceService
    {
        Task<Choice> CreateChoice(ChoiceDTO choiceDto, string userId);
        Task<Choice> UpdateChoice(string id, ChoiceDTO choiceDto, string userId);
        Task DeleteChoice(string id);
        Task<Choice> GetChoiceById(string id);
        Task<List<Choice>> GetChoices();
        Task<List<Choice>> GetChoicesByQuestionId(string questionId);
        Task<List<Choice>> GetChoicesBySurveyId(string surveyId);
    }
}

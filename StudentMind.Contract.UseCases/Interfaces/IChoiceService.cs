using StudentMind.Core.Entity;
using StudentMind.Services.DTO;

namespace StudentMind.Services.Interfaces
{
    public interface IChoiceService
    {
        Task<Choice> CreateChoice(ChoiceDTO choiceDto);
        Task<Choice> UpdateChoice(string id, ChoiceDTO choiceDto);
        Task DeleteChoice(string id);
        Task<Choice> GetChoiceById(string id);
        Task<List<Choice>> GetChoices();
    }
}

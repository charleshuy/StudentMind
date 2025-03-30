using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Services.Services
{
    public class ChoiceService : IChoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Choice> CreateChoice(ChoiceDTO choiceDto)
        {
            var ChoiceRepo = _unitOfWork.GetRepository<Choice>();
            var Choice = new Choice
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                Content = choiceDto.Content,
                QuestionId = choiceDto.QuestionId,
                Point = choiceDto.Point,
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now
            };
            await ChoiceRepo.InsertAsync(Choice);
            await _unitOfWork.SaveAsync();
            return Choice;
        }

        public async Task DeleteChoice(string id)
        {
            var ChoiceRepo = _unitOfWork.GetRepository<Choice>();
            Choice Choice = ChoiceRepo.GetById(id);
            await ChoiceRepo.DeleteAsync(Choice);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Choice> GetChoiceById(string id)
        {
            var ChoiceRepo = _unitOfWork.GetRepository<Choice>();
            return await ChoiceRepo.Entities.Include(c => c.Question).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Choice>> GetChoices()
        {
            var ChoiceRepo = _unitOfWork.GetRepository<Choice>();
            return ChoiceRepo.Entities.Include(c => c.Question).ToList();
        }

        public async Task<Choice> UpdateChoice(string id, ChoiceDTO choiceDto)
        {
            var ChoiceRepo = _unitOfWork.GetRepository<Choice>();
            var Choice = await ChoiceRepo.GetByIdAsync(id);
            Choice.Content = choiceDto.Content;
            Choice.QuestionId = choiceDto.QuestionId;
            Choice.Point = choiceDto.Point;
            Choice.LastUpdatedTime = DateTime.Now;
            await ChoiceRepo.UpdateAsync(Choice);
            await _unitOfWork.SaveAsync();
            return Choice;
        }
    }
}

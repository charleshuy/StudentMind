using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<Question> CreateQuestion(QuestionDTO questionDto);
        Task<Question> UpdateQuestion(string id, QuestionDTO questionDto);
        Task DeleteQuestion(string id);
        Task<Question> GetQuestionById(string id);
        Task<List<Question>> GetQuestions();
    }
}

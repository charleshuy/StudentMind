using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.QuestionPages
{
    public class IndexModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public IndexModel(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public IList<Question> Question { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Question = await _questionService.GetQuestions();
        }
    }
}

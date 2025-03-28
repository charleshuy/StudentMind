using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.QuestionPages
{
    public class DetailsModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public DetailsModel(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public Question Question { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _questionService.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }
            else
            {
                Question = question;
            }
            return Page();
        }
    }
}

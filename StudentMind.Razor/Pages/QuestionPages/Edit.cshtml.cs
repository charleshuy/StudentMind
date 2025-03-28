using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.QuestionPages
{
    public class EditModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public EditModel(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [BindProperty]
        public Question Question { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question =  await _questionService.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }
            Question = question;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                QuestionDTO questionDto = new QuestionDTO
                {
                    Content = Question.Content
                };
                await _questionService.UpdateQuestion(Question.Id, questionDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await QuestionExists(Question.Id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> QuestionExists(string id)
        {
            return await _questionService.GetQuestionById(id) != null;
        }
    }
}

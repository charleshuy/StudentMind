using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.ChoicePages
{
    public class CreateModel : PageModel
    {
        private readonly IChoiceService _choiceService;
        private readonly IQuestionService _questionService;

        public CreateModel(IChoiceService choiceService, IQuestionService questionService)
        {
            _choiceService = choiceService;
            _questionService = questionService;
        }

        public async Task<IActionResult> OnGet()
        {
            ViewData["QuestionId"] = new SelectList(await _questionService.GetQuestions(), "Id", "Content");
            return Page();
        }

        [BindProperty]
        public Choice Choice { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ChoiceDTO choiceDTO = new ChoiceDTO
            {
                Content = Choice.Content,
                QuestionId = Choice.QuestionId,
                Point = Choice.Point,
            };
            await _choiceService.CreateChoice(choiceDTO);

            return RedirectToPage("./Index");
        }
    }
}

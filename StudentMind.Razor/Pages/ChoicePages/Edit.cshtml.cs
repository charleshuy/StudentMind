using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.ChoicePages
{
    public class EditModel : PageModel
    {
        private readonly IChoiceService _choiceService;
        private readonly IQuestionService _questionService;

        public EditModel(IChoiceService choiceService, IQuestionService questionService)
        {
            _choiceService = choiceService;
            _questionService = questionService;
        }

        [BindProperty]
        public Choice Choice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var choice =  await _choiceService.GetChoiceById(id);
            if (choice == null)
            {
                return NotFound();
            }
            Choice = choice;
            ViewData["QuestionId"] = new SelectList(await _questionService.GetQuestions(), "Id", "Content");
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
                ChoiceDTO choiceDTO = new ChoiceDTO
                {
                    Content = Choice.Content,
                    QuestionId = Choice.QuestionId,
                    Point = Choice.Point,
                };
                await _choiceService.UpdateChoice(Choice.Id, choiceDTO);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await ChoiceExists(Choice.Id)))
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

        private async Task<bool> ChoiceExists(string id)
        {
            return _choiceService.GetChoiceById(id) != null;
        }
    }
}

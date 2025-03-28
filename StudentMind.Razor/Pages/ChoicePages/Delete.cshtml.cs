using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.ChoicePages
{
    public class DeleteModel : PageModel
    {
        private readonly IChoiceService _choiceService;

        public DeleteModel(IChoiceService choiceService)
        {
            _choiceService = choiceService;
        }

        [BindProperty]
        public Choice Choice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var choice = await _choiceService.GetChoiceById(id);

            if (choice == null)
            {
                return NotFound();
            }
            else
            {
                Choice = choice;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var choice = await _choiceService.GetChoiceById(id);
            if (choice != null)
            {
                Choice = choice;
                await _choiceService.DeleteChoice(Choice.Id);
            }

            return RedirectToPage("./Index");
        }
    }
}

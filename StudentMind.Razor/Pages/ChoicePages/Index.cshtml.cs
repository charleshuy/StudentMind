using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.ChoicePages
{
    public class IndexModel : PageModel
    {
        private readonly IChoiceService _choiceService;

        public IndexModel(IChoiceService choiceService)
        {
            _choiceService = choiceService;
        }

        public IList<Choice> Choice { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Choice = await _choiceService.GetChoices();
        }
    }
}

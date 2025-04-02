using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.ChoicePages
{
    public class IndexModel : PageModel
    {
        private readonly IChoiceService _choiceService;
        private const int PageSize = 4;

        public IndexModel(IChoiceService choiceService)
        {
            _choiceService = choiceService;
        }

        public IList<Choice> Choice { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchChoices { get; set; } = string.Empty;

        public async Task OnGetAsync(int? pageNumber)
        {
            CurrentPage = pageNumber ?? 1;

            var choices = await _choiceService.GetChoices();

            if (!string.IsNullOrEmpty(SearchChoices))
            {
                choices = choices.Where(m =>
                    (!string.IsNullOrEmpty(SearchChoices) && m.Content.Contains(SearchChoices, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            int totalRecords = choices.Count;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            Choice = choices
                .OrderBy(c => c.Question.Content) 
                .ThenBy(c => c.Point)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}

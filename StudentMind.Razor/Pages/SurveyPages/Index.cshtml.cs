using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.SurveyPages
{
    public class IndexModel : PageModel
    {
        private readonly ISurveyService _surveyService;
        private const int PageSize = 4;

        public IndexModel(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public IList<Survey> Survey { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchSurveys { get; set; } = string.Empty;

        public async Task OnGetAsync(int? pageNumber)
        {
            CurrentPage = pageNumber ?? 1;

            var surveys = await _surveyService.GetSurveys();
            if (!string.IsNullOrEmpty(SearchSurveys))
            {
                surveys = surveys.Where(m =>
                    (!string.IsNullOrEmpty(SearchSurveys) && m.Name.Contains(SearchSurveys, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            int totalRecords = surveys.Count;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            Survey = surveys.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.SurveyQuestionPages
{
    public class IndexModel : PageModel
    {
        private readonly ISurveyQuestionService _surveyQuestionService;
        private const int PageSize = 4;

        public IndexModel(ISurveyQuestionService surveyQuestionService)
        {
            _surveyQuestionService = surveyQuestionService;
        }

        public IList<SurveyQuestion> SurveyQuestion { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchQuestions { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string SearchSurveys { get; set; } = string.Empty;

        public async Task OnGetAsync(int? pageNumber)
        {
            CurrentPage = pageNumber ?? 1;
            var surveyQuestions = await _surveyQuestionService.GetSurveyQuestions();
            if (!string.IsNullOrEmpty(SearchSurveys))
            {
                surveyQuestions = surveyQuestions.Where(m =>
                    (!string.IsNullOrEmpty(SearchSurveys) && m.Survey.Name.Contains(SearchSurveys, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            if (!string.IsNullOrEmpty(SearchQuestions))
            {
                surveyQuestions = surveyQuestions.Where(m =>
                    (!string.IsNullOrEmpty(SearchQuestions) && m.Question.Content.Contains(SearchQuestions, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            int totalRecords = surveyQuestions.Count;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            SurveyQuestion = surveyQuestions.Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize).ToList();
        }
    }
}

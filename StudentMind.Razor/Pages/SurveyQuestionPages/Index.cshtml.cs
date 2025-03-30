using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.SurveyQuestionPages
{
    public class IndexModel : PageModel
    {
        private readonly ISurveyQuestionService _surveyQuestionService;

        public IndexModel(ISurveyQuestionService surveyQuestionService)
        {
            _surveyQuestionService = surveyQuestionService;
        }

        public IList<SurveyQuestion> SurveyQuestion { get;set; } = default!;

        public async Task OnGetAsync()
        {
            SurveyQuestion = await _surveyQuestionService.GetSurveyQuestions();
        }
    }
}

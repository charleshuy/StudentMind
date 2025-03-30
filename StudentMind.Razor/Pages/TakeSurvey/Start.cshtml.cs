using Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.TakeSurvey
{
    public class StartModel : PageModel
    {
        private readonly ISurveyService _surveyService;
        private readonly IQuestionService _questionService;
        private readonly ISurveyQuestionService _surveyQuestionService;
        private readonly IChoiceService _choiceService;

        public StartModel(ISurveyService surveyService, IQuestionService questionService, ISurveyQuestionService surveyQuestionService, IChoiceService choiceService)
        {
            _surveyService = surveyService;
            _questionService = questionService;
            _surveyQuestionService = surveyQuestionService;
            _choiceService = choiceService;
        }

        public Survey Survey { get; set; }
        public List<Question> Questions { get; set; }
        public List<Choice> Choices {  get; set; }

        [BindProperty]
        public Dictionary<string, string> Answers { get; set; } // QuestionId -> ChoiceId

        public async Task<IActionResult> OnGetAsync(string surveyId)
        {
            Survey = await _surveyService.GetSurveyById(surveyId);

            if (Survey == null)
            {
                return NotFound();
            }

            Questions = await _surveyQuestionService.GetQuestionsBySurveyId(surveyId);
            Choices = await _choiceService.GetChoicesBySurveyId(surveyId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve the survey again
            string surveyId = Request.Form["SurveyId"];
            Survey = await _surveyService.GetSurveyById(surveyId);

            if (Survey == null)
            {
                return NotFound();
            }

            // Store Answers in TempData as JSON (TempData only supports simple types)
            TempData["Answers"] = System.Text.Json.JsonSerializer.Serialize(Answers);

            return RedirectToPage("Submit", new { surveyId = Survey.Id });
        }
    }
}

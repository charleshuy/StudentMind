using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.SurveyQuestionPages
{
    public class CreateModel : PageModel
    {
        private readonly ISurveyQuestionService _surveyQuestionService;
        private readonly ISurveyService _surveyService;
        private readonly IQuestionService _questionService;

        public CreateModel(ISurveyQuestionService surveyQuestionService, ISurveyService surveyService, IQuestionService questionService)
        {
            _surveyQuestionService = surveyQuestionService;
            _surveyService = surveyService;
            _questionService = questionService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["SurveyId"] = new SelectList(await _surveyService.GetSurveys(), "Id", "Name");
            ViewData["QuestionId"] = new SelectList(await _questionService.GetQuestions(), "Id", "Content");
            return Page();
        }

        [BindProperty]
        public SurveyQuestion SurveyQuestion { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Console.WriteLine("Survey: " + SurveyQuestion.SurveyId);
            Console.WriteLine("Question: " + SurveyQuestion.QuestionId);
            SurveyQuestionDTO surveyQuestionDTO = new SurveyQuestionDTO
            {
                SurveyId = SurveyQuestion.SurveyId,
                QuestionId = SurveyQuestion.QuestionId
            };
            await _surveyQuestionService.CreateSurveyQuestion(surveyQuestionDTO);

            return RedirectToPage("./Index");
        }
    }
}

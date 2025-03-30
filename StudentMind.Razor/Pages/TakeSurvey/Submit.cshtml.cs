using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.TakeSurvey
{
    public class SubmitModel : PageModel
    {
        private readonly ISurveyResponseService _surveyResponseService;
        private readonly IChoiceService _choiceService;
        private readonly IHealthScoreRuleService _healthScoreRuleService;
        private readonly IStudentHealthService _studentHealthService;

        public SubmitModel(
            ISurveyResponseService surveyResponseService,
            IChoiceService choiceService,
            IHealthScoreRuleService healthScoreRuleService,
            IStudentHealthService studentHealthService)
        {
            _surveyResponseService = surveyResponseService;
            _choiceService = choiceService;
            _healthScoreRuleService = healthScoreRuleService;
            _studentHealthService = studentHealthService;
        }

        public int TotalScore { get; set; }
        public string HealthCategory { get; set; }

        [BindProperty]
        public Dictionary<string, string> Answers { get; set; }

        public async Task<IActionResult> OnGetAsync(string surveyId)
        {
            if (TempData["Answers"] != null)
            {
                Answers = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>((string)TempData["Answers"]);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string surveyId, Dictionary<string, string> Answers)
        {
            if (string.IsNullOrEmpty(surveyId) || Answers == null || !Answers.Any())
            {
                ModelState.AddModelError(string.Empty, "Invalid survey submission.");
                return Page();
            }

            var studentId = User.Identity?.Name; // Ensure authentication is implemented

            // Get selected choices
            var choiceIds = Answers.Values.ToList();
            var selectedChoices = await _choiceService.GetChoicesByIds(choiceIds);

            // Calculate total score
            TotalScore = selectedChoices.Sum(c => c.Point);

            // Get health category based on score
            var rule = await _healthScoreRuleService.GetRuleByScore(TotalScore);
            HealthCategory = rule?.category ?? "Unknown";

            // Create SurveyResponse for each question-answer pair
            foreach (var (questionId, choiceId) in Answers)
            {
                var surveyResponseDto = new SurveyResponseDTO
                {
                    UserId = studentId,
                    SurveyId = surveyId,
                    QuestionId = questionId,
                    ChoiceId = choiceId
                };
                await _surveyResponseService.CreateSurveyResponse(surveyResponseDto);
            }

            // Create StudentHealth record
            var studentHealthDto = new StudentHealthDTO
            {
                StudentId = studentId,
                SurveyId = surveyId,
                Score = TotalScore,
                Category = HealthCategory
            };
            await _studentHealthService.CreateStudentHealth(studentHealthDto);

            return Page();
        }
    }
}
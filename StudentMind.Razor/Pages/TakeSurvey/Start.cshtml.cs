using Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;
using StudentMind.Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentMind.Razor.Pages.TakeSurvey
{
    public class StartModel : PageModel
    {
        private readonly ISurveyService _surveyService;
        private readonly IQuestionService _questionService;
        private readonly ISurveyQuestionService _surveyQuestionService;
        private readonly IChoiceService _choiceService;
        private readonly ISurveyResponseService _surveyResponseService;
        private readonly IHealthScoreRuleService _healthScoreRuleService;
        private readonly IStudentHealthService _studentHealthService;

        public StartModel(ISurveyService surveyService, IQuestionService questionService, ISurveyQuestionService surveyQuestionService, IChoiceService choiceService, ISurveyResponseService surveyResponseService, IHealthScoreRuleService healthScoreRuleService, IStudentHealthService studentHealthService)
        {
            _surveyService = surveyService;
            _questionService = questionService;
            _surveyQuestionService = surveyQuestionService;
            _choiceService = choiceService;
            _surveyResponseService = surveyResponseService;
            _healthScoreRuleService = healthScoreRuleService;
            _studentHealthService = studentHealthService;
        }

        [BindProperty(SupportsGet = true)]
        public string SurveyId { get; set; }
        public Survey Survey { get; set; }
        public List<Question> Questions { get; set; }
        public List<Choice> Choices { get; set; }
        public string userId { get; set; }

        [BindProperty]
        public Dictionary<string, string> Answers { get; set; } // QuestionId -> ChoiceId

        public async Task<IActionResult> OnGetAsync(string surveyId)
        {
            var token = HttpContext.Session.GetString("JWT_Token");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }

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

            var token = HttpContext.Session.GetString("JWT_Token");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }

            List<Choice> selectedChoices = new();
            foreach (var (questionId, choiceId) in Answers)
            {
                Choice choice = await _choiceService.GetChoiceById(choiceId);
                selectedChoices.Add(choice);
            }

            foreach (var choice in selectedChoices)
            {
                Console.WriteLine("Choice: " + choice.Content);
                SurveyResponseDTO surveyResponseDto = new SurveyResponseDTO
                {
                    ChoiceId = choice.Id,
                    QuestionId = choice.QuestionId,
                    SurveyId = SurveyId,
                    UserId = userId
                };
                await _surveyResponseService.CreateSurveyResponse(surveyResponseDto);
            }

            var existStudentHealth = await _studentHealthService.GetStudentHealthByUserIdSurveyId(userId, SurveyId);

            int totalPoints = selectedChoices.Sum(choice => choice.Point);
            HealthScoreRule studentHealthRule = await _healthScoreRuleService.GetRuleByScore(totalPoints);
            StudentHealthDTO studentHealthDTO = new StudentHealthDTO
            {
                StudentId = userId,
                SurveyId = SurveyId,
                Score = totalPoints,
                Category = studentHealthRule.category
            };

            if (existStudentHealth != null)
            {
                await _studentHealthService.UpdateStudentHealth(existStudentHealth.Id, studentHealthDTO);
            }
            else
            {
                await _studentHealthService.CreateStudentHealth(studentHealthDTO);
            }

            return RedirectToPage("Submit", new { surveyId = SurveyId, userId });
        }
    }
}
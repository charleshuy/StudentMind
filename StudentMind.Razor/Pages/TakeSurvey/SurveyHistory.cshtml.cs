using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentMind.Razor.Pages.TakeSurvey
{
    public class SurveyHistoryModel : PageModel
    {
        private readonly ISurveyService _surveyService;
        private readonly IQuestionService _questionService;
        private readonly ISurveyQuestionService _surveyQuestionService;
        private readonly IChoiceService _choiceService;
        private readonly ISurveyResponseService _surveyResponseService;

        public SurveyHistoryModel(
            ISurveyService surveyService,
            IQuestionService questionService,
            ISurveyQuestionService surveyQuestionService,
            IChoiceService choiceService,
            ISurveyResponseService surveyResponseService)
        {
            _surveyService = surveyService;
            _questionService = questionService;
            _surveyQuestionService = surveyQuestionService;
            _choiceService = choiceService;
            _surveyResponseService = surveyResponseService;
        }

        [BindProperty(SupportsGet = true)]
        public string SurveyId { get; set; }
        public Survey Survey { get; set; }
        public List<Question> Questions { get; set; }
        public List<Choice> Choices { get; set; }
        public Dictionary<string, string> Answers { get; set; } = new(); // QuestionId -> Selected ChoiceId
        public string UserId { get; private set; }

        public async Task<IActionResult> OnGetAsync(string surveyId)
        {
            if (string.IsNullOrEmpty(surveyId))
            {
                return BadRequest("Survey ID is required.");
            }

            SurveyId = surveyId;
            UserId = GetUserIdFromToken();

            Survey = await _surveyService.GetSurveyById(SurveyId);
            if (Survey == null)
            {
                return NotFound();
            }

            Questions = await _surveyQuestionService.GetQuestionsBySurveyId(SurveyId);
            Choices = await _choiceService.GetChoicesBySurveyId(SurveyId);

            // Load selected answers
            var responses = await _surveyResponseService.GetSurveyResponsesByUserIdSurveyId(UserId, SurveyId);
            Answers = responses.ToDictionary(r => r.QuestionId, r => r.ChoiceId);
            foreach(var (questionId, choiceId) in Answers)
            {
                Console.WriteLine(questionId + " : " + choiceId);
            }

            return Page();
        }

        public string GetUserIdFromToken()
        {
            var jwtToken = Request.Cookies["JWT_Token"];

            if (string.IsNullOrEmpty(jwtToken))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);

            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }
    }
}

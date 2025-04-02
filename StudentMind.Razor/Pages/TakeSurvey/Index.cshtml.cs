using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;
using System.Security.Claims;

namespace StudentMind.Razor.Pages.TakeSurvey
{
    public class IndexModel : PageModel
    {
        private readonly ISurveyService _surveyService;
        private readonly ISurveyResponseService _surveyResponseService;

        public IndexModel(ISurveyService surveyService, ISurveyResponseService surveyResponseService)
        {
            _surveyService = surveyService;
            _surveyResponseService = surveyResponseService;
        }

        public List<Survey> Surveys { get; set; }
        public List<string> SurveysAlreadyTaken { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            // Fetch surveys
            Surveys = await _surveyService.GetSurveys();

            // Get the current user's ID (assumed from JWT or cookies)
            var userId = GetUserIdFromToken();
            if (!string.IsNullOrEmpty(userId))
            {
                // Check if user has completed any survey
                foreach (var survey in Surveys)
                {
                    var responses = await _surveyResponseService.GetSurveyResponsesByUserIdSurveyId(userId, survey.Id);
                    if (responses.Any())
                    {
                        SurveysAlreadyTaken.Add(survey.Id); // Track surveys already taken
                    }
                }
            }
        }

        private string GetUserIdFromToken()
        {
            var jwtToken = Request.Cookies["JWT_Token"];

            if (string.IsNullOrEmpty(jwtToken))
                return null;

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);

            return token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}

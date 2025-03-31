using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.TakeSurvey
{
    public class SubmitModel : PageModel
    {
        private readonly IStudentHealthService _studentHealthService;

        public SubmitModel(IStudentHealthService studentHealthService)
        {
            _studentHealthService = studentHealthService;
        }

        public int TotalScore { get; set; }
        public string HealthCategory { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string surveyId, string userId)
        {
            StudentHealth studentHealth = await _studentHealthService.GetStudentHealthByUserIdSurveyId(userId, surveyId);
            if (studentHealth == null)
            {
                ErrorMessage = "No survey results found for you. Please try take the survey again.";
                return Page();
            }

            TotalScore = studentHealth.Score;
            HealthCategory = studentHealth.Category;
            return Page();
        }
    }
}
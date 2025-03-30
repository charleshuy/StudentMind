using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.HealthScoreRulePages
{
    public class CreateModel : PageModel
    {
        private readonly IHealthScoreRuleService _healthScoreRuleService;
        private readonly ISurveyService _surveyService;

        public CreateModel(IHealthScoreRuleService healthScoreRuleService, ISurveyService surveyService)
        {
            _healthScoreRuleService = healthScoreRuleService;
            _surveyService = surveyService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["SurveyId"] = new SelectList(await _surveyService.GetSurveys(), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public HealthScoreRule HealthScoreRule { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HealthScoreRuleDTO healthScoreRuleDto = new HealthScoreRuleDTO
            {
                SurveyId = HealthScoreRule.SurveyId,
                MinScore = HealthScoreRule.MinScore,
                MaxScore = HealthScoreRule.MaxScore,
                category = HealthScoreRule.category
            };
            await _healthScoreRuleService.CreateHealthScoreRule(healthScoreRuleDto);

            return RedirectToPage("./Index");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.HealthScoreRulePages
{
    public class EditModel : PageModel
    {
        private readonly IHealthScoreRuleService _healthScoreRuleService;
        private readonly ISurveyService _surveyService;

        public EditModel(IHealthScoreRuleService healthScoreRuleService, ISurveyService surveyService)
        {
            _healthScoreRuleService = healthScoreRuleService;
            _surveyService = surveyService;
        }

        [BindProperty]
        public HealthScoreRule HealthScoreRule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthscorerule =  await _healthScoreRuleService.GetHealthScoreRuleById(id);
            if (healthscorerule == null)
            {
                return NotFound();
            }
            HealthScoreRule = healthscorerule;
            ViewData["SurveyId"] = new SelectList(await _surveyService.GetSurveys(), "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                HealthScoreRuleDTO healthScoreRuleDto = new HealthScoreRuleDTO
                {
                    SurveyId = HealthScoreRule.SurveyId,
                    MinScore = HealthScoreRule.MinScore,
                    MaxScore = HealthScoreRule.MaxScore,
                    category = HealthScoreRule.category
                };
                await _healthScoreRuleService.UpdateHealthScoreRule(HealthScoreRule.Id, healthScoreRuleDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await HealthScoreRuleExists(HealthScoreRule.Id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> HealthScoreRuleExists(string id)
        {
            return await _healthScoreRuleService.GetHealthScoreRuleById(id) != null;
        }
    }
}

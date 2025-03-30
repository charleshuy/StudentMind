using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.HealthScoreRulePages
{
    public class DetailsModel : PageModel
    {
        private readonly IHealthScoreRuleService _healthScoreRuleService;

        public DetailsModel(IHealthScoreRuleService healthScoreRuleService)
        {
            _healthScoreRuleService = healthScoreRuleService;
        }

        public HealthScoreRule HealthScoreRule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthscorerule = await _healthScoreRuleService.GetHealthScoreRuleById(id);
            if (healthscorerule == null)
            {
                return NotFound();
            }
            else
            {
                HealthScoreRule = healthscorerule;
            }
            return Page();
        }
    }
}

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
    public class IndexModel : PageModel
    {
        private readonly IHealthScoreRuleService _healthScoreRuleService;

        public IndexModel(IHealthScoreRuleService healthScoreRuleService)
        {
            _healthScoreRuleService = healthScoreRuleService;
        }

        public IList<HealthScoreRule> HealthScoreRule { get;set; } = default!;

        public async Task OnGetAsync()
        {
            HealthScoreRule = await _healthScoreRuleService.GetHealthScoreRules();
        }
    }
}

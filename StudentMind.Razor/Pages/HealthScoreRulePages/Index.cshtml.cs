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
        private const int PageSize = 4;

        public IndexModel(IHealthScoreRuleService healthScoreRuleService)
        {
            _healthScoreRuleService = healthScoreRuleService;
        }

        public IList<HealthScoreRule> HealthScoreRule { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchHealthScores { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var healthScoreRules = await _healthScoreRuleService.GetHealthScoreRules();
            if (!string.IsNullOrEmpty(SearchHealthScores))
            {
                healthScoreRules = healthScoreRules.Where(m =>
                    (!string.IsNullOrEmpty(SearchHealthScores) && m.category.Contains(SearchHealthScores, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            int totalRecords = healthScoreRules.Count;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);
            HealthScoreRule = healthScoreRules.OrderBy(h => h.MinScore).Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize).ToList();
        }
    }
}

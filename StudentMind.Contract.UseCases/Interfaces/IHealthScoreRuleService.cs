using StudentMind.Core.Entity;
using StudentMind.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.Interfaces
{
    public interface IHealthScoreRuleService
    {
        Task<HealthScoreRule> CreateHealthScoreRule(HealthScoreRuleDTO healthScoreRuleDto);
        Task<HealthScoreRule> UpdateHealthScoreRule(string id, HealthScoreRuleDTO healthScoreRuleDto);
        Task DeleteHealthScoreRule(string id);
        Task<HealthScoreRule> GetHealthScoreRuleById(string id);
        Task<List<HealthScoreRule>> GetHealthScoreRules();
        Task<HealthScoreRule> GetRuleByScore(int score);
    }
}

using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.Services
{
    public class HealthScoreRuleService : IHealthScoreRuleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HealthScoreRuleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HealthScoreRule> CreateHealthScoreRule(HealthScoreRuleDTO healthScoreRuleDto)
        {
            var healthScoreRuleRepo = _unitOfWork.GetRepository<HealthScoreRule>();
            var healthScoreRule = new HealthScoreRule
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                SurveyId = healthScoreRuleDto.SurveyId,
                MinScore = healthScoreRuleDto.MinScore,
                MaxScore = healthScoreRuleDto.MaxScore,
                category = healthScoreRuleDto.category,
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now
            };
            await healthScoreRuleRepo.InsertAsync(healthScoreRule);
            await _unitOfWork.SaveAsync();
            return healthScoreRule;
        }

        public async Task DeleteHealthScoreRule(string id)
        {
            var healthScoreRuleRepo = _unitOfWork.GetRepository<HealthScoreRule>();
            HealthScoreRule healthScoreRule = healthScoreRuleRepo.GetById(id);
            await healthScoreRuleRepo.DeleteAsync(healthScoreRule);
            await _unitOfWork.SaveAsync();
        }

        public async Task<HealthScoreRule> GetHealthScoreRuleById(string id)
        {
            var healthScoreRuleRepo = _unitOfWork.GetRepository<HealthScoreRule>();
            return await healthScoreRuleRepo.Entities.Include(hs => hs.Survey).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<HealthScoreRule>> GetHealthScoreRules()
        {
            var healthScoreRuleRepo = _unitOfWork.GetRepository<HealthScoreRule>();
            return healthScoreRuleRepo.Entities.Include(hs => hs.Survey).ToList();
        }

        public async Task<HealthScoreRule> UpdateHealthScoreRule(string id, HealthScoreRuleDTO healthScoreRuleDto)
        {
            var healthScoreRuleRepo = _unitOfWork.GetRepository<HealthScoreRule>();
            var healthScoreRule = await healthScoreRuleRepo.GetByIdAsync(id);
            healthScoreRule.SurveyId = healthScoreRuleDto.SurveyId;
            healthScoreRule.MinScore = healthScoreRuleDto.MinScore;
            healthScoreRule.MaxScore = healthScoreRuleDto.MaxScore;
            healthScoreRule.category = healthScoreRuleDto.category;
            healthScoreRule.LastUpdatedTime = DateTime.Now;
            await healthScoreRuleRepo.UpdateAsync(healthScoreRule);
            await _unitOfWork.SaveAsync();
            return healthScoreRule;
        }

        public async Task<HealthScoreRule> GetRuleByScore(int score)
        {
            var healthScoreRuleRepo = _unitOfWork.GetRepository<HealthScoreRule>();
            return await healthScoreRuleRepo.Entities
                .Where(rule => score >= rule.MinScore && score <= rule.MaxScore)
                .FirstOrDefaultAsync();
        }
    }
}

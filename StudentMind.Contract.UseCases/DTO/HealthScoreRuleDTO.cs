using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.DTO
{
    public class HealthScoreRuleDTO
    {
        public string SurveyId { get; set; }
        public int MinScore { get; set; }
        public int MaxScore { get; set; }
        public string category { get; set; }
    }
}

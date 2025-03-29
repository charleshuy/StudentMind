using StudentMind.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Core.Entity
{
    public class HealthScoreRule : BaseEntity
    {
        public string SurveyId { get; set; }
        public int MinScore { get; set; }
        public int MaxScore { get; set; }
        public string category { get; set; }
        public Survey? Survey { get; set; }
    }
}

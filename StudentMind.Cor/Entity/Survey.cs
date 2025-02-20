using StudentMind.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Core.Entity
{
    public class Survey : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalParticipants { get; set; }
        public string TypeId { get; set; }

        public SurveyType Type { get; set; }
        public ICollection<SurveyResponse> SurveyResponses { get; set; }
        public ICollection<SurveyQuestion> SurveyQuestions { get; set; }
    }
}

using StudentMind.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Core.Entity
{
    public class Survey : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Total Participants is required")]
        public int TotalParticipants { get; set; }
        [Required(ErrorMessage = "Type id is required")]
        public string? TypeId { get; set; }

        public SurveyType? Type { get; set; }
        public ICollection<SurveyResponse>? SurveyResponses { get; set; }
        public ICollection<SurveyQuestion>? SurveyQuestions { get; set; }
        public ICollection<HealthScoreRule>? HealthScoreRules { get; set; }
        public virtual ICollection<StudentHealth>? StudentHealths { get; set; } = new List<StudentHealth>();
    }
}

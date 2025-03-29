using StudentMind.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace StudentMind.Core.Entity
{
    public class Question : BaseEntity
    {
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public ICollection<Choice>? Choices { get; set; }
        public ICollection<SurveyQuestion>? SurveyQuestions { get; set; }
        public ICollection<SurveyResponse>? SurveyResponses { get; set; }
    }
}

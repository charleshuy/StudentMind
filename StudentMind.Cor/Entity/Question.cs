using StudentMind.Core.Base;

namespace StudentMind.Core.Entity
{
    public class Question : BaseEntity
    {
        public string? Content { get; set; }

        public ICollection<Choice>? Choices { get; set; }
        public ICollection<SurveyQuestion>? SurveyQuestions { get; set; }
    }
}

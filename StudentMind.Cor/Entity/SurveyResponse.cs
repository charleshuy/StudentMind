using StudentMind.Core.Base;


namespace StudentMind.Core.Entity
{
    public class SurveyResponse : BaseEntity
    {
        public string SurveyId { get; set; }
        public string UserId { get; set; }
        public string QuestionId { get; set; }
        public string ChoiceId { get; set; }

        public Survey? Survey { get; set; }
        public User? User { get; set; }
        public Question? Question { get; set; }
        public Choice? Choice { get; set; }
    }
}

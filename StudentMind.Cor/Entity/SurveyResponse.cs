using StudentMind.Core.Base;


namespace StudentMind.Core.Entity
{
    public class SurveyResponse : BaseEntity
    {
        public string SurveyId { get; set; }
        public string UserId { get; set; }

        public Survey Survey { get; set; }
        public User User { get; set; }
    }
}

using StudentMind.Core.Base;


namespace StudentMind.Core.Entity
{
    public class SurveyType : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Survey> Surveys { get; set; }
    }
}

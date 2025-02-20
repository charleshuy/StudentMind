using StudentMind.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Core.Entity
{
    public class Question : BaseEntity
    {
        public long Content { get; set; }

        public ICollection<Choice> Choices { get; set; }
        public ICollection<SurveyQuestion> SurveyQuestions { get; set; }
    }
}

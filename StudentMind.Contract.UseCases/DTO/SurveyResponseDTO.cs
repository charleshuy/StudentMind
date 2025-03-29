using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.DTO
{
    public class SurveyResponseDTO
    {
        public string SurveyId { get; set; }
        public string UserId { get; set; }
        public string QuestionId { get; set; }
        public string ChoiceId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.DTO
{
    public class StudentHealthDTO
    {
        public string StudentId { get; set; }
        public string SurveyId { get; set; }
        public int Score { get; set; }
        public string Category { get; set; }
    }
}

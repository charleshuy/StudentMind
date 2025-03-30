using StudentMind.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Core.Entity
{
    public class StudentHealth : BaseEntity
    {
        public string StudentId { get; set; }
        public string SurveyId { get; set; }
        public int Score { get; set; }
        public string Category { get; set; }

        public Survey? Survey { get; set; }
        public User? Student { get; set; }
    }
}

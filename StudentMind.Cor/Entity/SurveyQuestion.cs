﻿using StudentMind.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Core.Entity
{
    public class SurveyQuestion : BaseEntity
    {
        public string SurveyId { get; set; }
        public string QuestionId { get; set; }

        public Survey? Survey { get; set; }
        public Question? Question { get; set; }
    }

}

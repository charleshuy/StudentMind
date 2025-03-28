using StudentMind.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.Interfaces
{
    public interface ISurveyTypeService
    {
        Task<List<SurveyType>> GetSurveyTypes();
    }
}

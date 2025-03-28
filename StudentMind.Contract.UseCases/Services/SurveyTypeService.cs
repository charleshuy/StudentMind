using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMind.Services.Services
{
    public class SurveyTypeService : ISurveyTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SurveyTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SurveyType>> GetSurveyTypes()
        {
            var surveyTypeRepo = _unitOfWork.GetRepository<SurveyType>();
            var surveyTypes = await surveyTypeRepo.GetAllAsync();
            return surveyTypes.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.SurveyResponsePages
{
    public class IndexModel : PageModel
    {
        private readonly ISurveyResponseService _surveyResponseService;
        private const int PageSize = 4;

        public IndexModel(ISurveyResponseService surveyResponseService)
        {
            _surveyResponseService = surveyResponseService;
        }

        public IList<SurveyResponse> SurveyResponse { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int? pageNumber)
        {
            CurrentPage = pageNumber ?? 1;
            var surveyResponses = await _surveyResponseService.GetSurveyResponses();
            int totalRecords = surveyResponses.Count;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);
            SurveyResponse = surveyResponses.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}

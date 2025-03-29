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

namespace StudentMind.Razor.Pages.SurveyQuestionPages
{
    public class DeleteModel : PageModel
    {
        private readonly ISurveyQuestionService _surveyQuestionService;

        public DeleteModel(ISurveyQuestionService surveyQuestionService)
        {
            _surveyQuestionService = surveyQuestionService;
        }

        [BindProperty]
        public SurveyQuestion SurveyQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyquestion = await _surveyQuestionService.GetSurveyQuestionById(id);

            if (surveyquestion == null)
            {
                return NotFound();
            }
            else
            {
                SurveyQuestion = surveyquestion;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyquestion = await _surveyQuestionService.GetSurveyQuestionById(id);
            if (surveyquestion != null)
            {
                SurveyQuestion = surveyquestion;
                await _surveyQuestionService.DeleteSurveyQuestion(SurveyQuestion.Id);
            }

            return RedirectToPage("./Index");
        }
    }
}

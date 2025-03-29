using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.SurveyQuestionPages
{
    public class EditModel : PageModel
    {
        private readonly ISurveyQuestionService _surveyQuestionService;
        private readonly ISurveyService _surveyService;
        private readonly IQuestionService _questionService;

        public EditModel(ISurveyQuestionService surveyQuestionService, ISurveyService surveyService, IQuestionService questionService)
        {
            _surveyQuestionService = surveyQuestionService;
            _surveyService = surveyService;
            _questionService = questionService;
        }

        [BindProperty]
        public SurveyQuestion SurveyQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyquestion =  await _surveyQuestionService.GetSurveyQuestionById(id);
            if (surveyquestion == null)
            {
                return NotFound();
            }
            SurveyQuestion = surveyquestion;
            ViewData["SurveyId"] = new SelectList(await _surveyService.GetSurveys(), "Id", "Name");
            ViewData["QuestionId"] = new SelectList(await _questionService.GetQuestions(), "Id", "Content");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                SurveyQuestionDTO surveyQuestionDTO = new SurveyQuestionDTO 
                {
                    SurveyId = SurveyQuestion.SurveyId,
                    QuestionId = SurveyQuestion.QuestionId
                };
                //await _surveyQuestionService.UpdateSurveyQuestion(SurveyQuestion.Id, surveyQuestionDTO);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await SurveyQuestionExists(SurveyQuestion.SurveyId)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> SurveyQuestionExists(string id)
        {
            return await _surveyQuestionService.GetSurveyQuestionById(id) != null;
        }
    }
}

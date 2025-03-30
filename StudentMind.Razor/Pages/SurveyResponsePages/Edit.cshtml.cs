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

namespace StudentMind.Razor.Pages.SurveyResponsePages
{
    public class EditModel : PageModel
    {
        private readonly ISurveyResponseService _surveyResponseService;
        private readonly IChoiceService _choiceService;
        private readonly IQuestionService _questionService;
        private readonly ISurveyService _surveyService;
        private readonly IUserService _userService;

        public EditModel(ISurveyResponseService surveyResponseService, IChoiceService choiceService, IQuestionService questionService, ISurveyService surveyService, IUserService userService)
        {
            _surveyResponseService = surveyResponseService;
            _choiceService = choiceService;
            _questionService = questionService;
            _surveyService = surveyService;
            _userService = userService;
        }

        [BindProperty]
        public SurveyResponse SurveyResponse { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyresponse =  await _surveyResponseService.GetSurveyResponseById(id);
            if (surveyresponse == null)
            {
                return NotFound();
            }
            SurveyResponse = surveyresponse;
            ViewData["ChoiceId"] = new SelectList(await _choiceService.GetChoices(), "Id", "Content");
            ViewData["QuestionId"] = new SelectList(await _questionService.GetQuestions(), "Id", "Content");
            ViewData["SurveyId"] = new SelectList(await _surveyService.GetSurveys(), "Id", "Name");
            ViewData["UserId"] = new SelectList(await _userService.GetAllUsersAsync(), "Id", "FullName");
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
                SurveyResponseDTO surveyResponseDto = new SurveyResponseDTO
                {
                    ChoiceId = SurveyResponse.ChoiceId,
                    QuestionId = SurveyResponse.QuestionId,
                    SurveyId = SurveyResponse.SurveyId,
                    UserId = SurveyResponse.UserId
                };
                await _surveyResponseService.UpdateSurveyResponse(SurveyResponse.Id, surveyResponseDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await SurveyResponseExists(SurveyResponse.Id)))
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

        private async Task<bool> SurveyResponseExists(string id)
        {
            return await _surveyResponseService.GetSurveyResponseById(id) != null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.SurveyResponsePages
{
    public class CreateModel : PageModel
    {
        private readonly ISurveyResponseService _surveyResponseService;
        private readonly IChoiceService _choiceService;
        private readonly IQuestionService _questionService;
        private readonly ISurveyService _surveyService;
        private readonly IUserService _userService;

        public CreateModel(ISurveyResponseService surveyResponseService, IChoiceService choiceService, IQuestionService questionService, ISurveyService surveyService, IUserService userService)
        {
            _surveyResponseService = surveyResponseService;
            _choiceService = choiceService;
            _questionService = questionService;
            _surveyService = surveyService;
            _userService = userService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ChoiceId"] = new SelectList(await _choiceService.GetChoices(), "Id", "Content");
            ViewData["QuestionId"] = new SelectList(await _questionService.GetQuestions(), "Id", "Content");
            ViewData["SurveyId"] = new SelectList(await _surveyService.GetSurveys(), "Id", "Name");
            ViewData["UserId"] = new SelectList(await _userService.GetAllUsersAsync(), "Id", "FullName");
            return Page();
        }

        [BindProperty]
        public SurveyResponse SurveyResponse { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            SurveyResponseDTO surveyResponseDto = new SurveyResponseDTO
            {
                ChoiceId = SurveyResponse.ChoiceId,
                QuestionId = SurveyResponse.QuestionId,
                SurveyId = SurveyResponse.SurveyId,
                UserId = SurveyResponse.UserId
            };
            await _surveyResponseService.CreateSurveyResponse(surveyResponseDto);

            return RedirectToPage("./Index");
        }
    }
}

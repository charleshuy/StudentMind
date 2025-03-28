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

namespace StudentMind.Razor.Pages.SurveyPages
{
    public class EditModel : PageModel
    {
        private readonly ISurveyService _surveyService;
        private readonly ISurveyTypeService _surveyTypeService;

        public EditModel(ISurveyService surveyService, ISurveyTypeService surveyTypeService)
        {
            _surveyService = surveyService;
            _surveyTypeService = surveyTypeService;
        }

        [BindProperty]
        public Survey Survey { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey =  await _surveyService.GetSurveyById(id);
            if (survey == null)
            {
                return NotFound();
            }
            Survey = survey;
            ViewData["TypeId"] = new SelectList(await _surveyTypeService.GetSurveyTypes(), "Id", "Name");
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
                SurveyDTO surveyDTO = new SurveyDTO
                {
                    Name = Survey.Name,
                    Description = Survey.Description,
                    TypeId = Survey.TypeId,
                    StartDate = Survey.StartDate,
                    EndDate = Survey.EndDate,
                    TotalParticipants = Survey.TotalParticipants
                };
                await _surveyService.UpdateSurvey(Survey.Id, surveyDTO);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await SurveyExists(Survey.Id)))
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

        private async Task<bool> SurveyExists(string id)
        {
            return await _surveyService.GetSurveyById(id) != null;
        }
    }
}

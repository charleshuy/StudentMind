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

namespace StudentMind.Razor.Pages.ChoicePages
{
    public class DetailsModel : PageModel
    {
        private readonly IChoiceService _choiceService;

        public DetailsModel(IChoiceService choiceService)
        {
            _choiceService = choiceService;
        }

        public Choice Choice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var choice = await _choiceService.GetChoiceById(id);
            if (choice == null)
            {
                return NotFound();
            }
            else
            {
                Choice = choice;
            }
            return Page();
        }
    }
}

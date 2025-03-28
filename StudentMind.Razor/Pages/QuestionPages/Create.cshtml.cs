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

namespace StudentMind.Razor.Pages.QuestionPages
{
    public class CreateModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public CreateModel(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Question Question { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            QuestionDTO question = new QuestionDTO
            {
                Content = Question.Content
            };
            await _questionService.CreateQuestion(question);

            return RedirectToPage("./Index");
        }
    }
}

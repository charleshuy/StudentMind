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

namespace StudentMind.Razor.Pages.QuestionPages
{
    public class IndexModel : PageModel
    {
        private readonly IQuestionService _questionService;
        private const int PageSize = 4;

        public IndexModel(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public IList<Question> Question { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchQuestions { get; set; } = string.Empty;

        public async Task OnGetAsync(int? pageNumber)
        {
            CurrentPage = pageNumber ?? 1;

            var questions = await _questionService.GetQuestions();
            if (!string.IsNullOrEmpty(SearchQuestions))
            {
                questions = questions.Where(m =>
                    (!string.IsNullOrEmpty(SearchQuestions) && m.Content.Contains(SearchQuestions, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            int totalRecords = questions.Count;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            Question = questions.Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize).ToList();
        }
    }
}

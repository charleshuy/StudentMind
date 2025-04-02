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

namespace StudentMind.Razor.Pages.StudentHealthPages
{
    public class IndexModel : PageModel
    {
        private readonly IStudentHealthService _studentHealthService;
        private const int PageSize = 4;

        public IndexModel(IStudentHealthService studentHealthService)
        {
            _studentHealthService = studentHealthService;
        }

        public IList<StudentHealth> StudentHealth { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int? pageNumber)
        {
            CurrentPage = pageNumber ?? 1;

            var studentHealths = await _studentHealthService.GetStudentHealths();
            int totalRecords = studentHealths.Count;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);
            StudentHealth = studentHealths.Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize).ToList();
        }
    }
}

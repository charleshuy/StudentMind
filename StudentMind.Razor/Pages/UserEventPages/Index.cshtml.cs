using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;

namespace StudentMind.Razor.Pages.UserEventPages
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IEnumerable<Event> Events { get; set; }

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task OnGetAsync()
        {
            var eventRepo = _unitOfWork.GetRepository<Event>();
            Events = await eventRepo.Entities
                .Include(e => e.Host)
                .Where(e => e.DeletedTime == null) // Only show non-deleted events
                .ToListAsync();
        }
    }
}
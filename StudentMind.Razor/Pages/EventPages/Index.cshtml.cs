using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.EventPages
{
    public class IndexModel : PageModel
    {
        private readonly IEventService _eventService;

        public IEnumerable<EventDTO> Events { get; set; }

        public IndexModel(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task OnGetAsync()
        {
            Events = await _eventService.GetAllEventsAsync();
        }
    }
}
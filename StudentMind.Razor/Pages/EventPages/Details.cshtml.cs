using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.EventPages
{
    public class DetailsModel : PageModel
    {
        private readonly IEventService _eventService;

        public EventDTO EventDTO { get; set; }

        public DetailsModel(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            EventDTO = await _eventService.GetEventByIdAsync(id);

            if (EventDTO == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
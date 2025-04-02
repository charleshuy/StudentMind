using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.EventPages
{
    public class CreateModel : PageModel
    {
        private readonly IEventService _eventService;

        [BindProperty]
        public EventDTO EventDTO { get; set; }

        public CreateModel(IEventService eventService)
        {
            _eventService = eventService;
        }

        public IActionResult OnGet()
        {
            EventDTO = new EventDTO();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _eventService.CreateEventAsync(EventDTO);
            return RedirectToPage("Index");
        }
    }
}
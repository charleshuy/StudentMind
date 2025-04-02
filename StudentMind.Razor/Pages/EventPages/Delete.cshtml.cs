using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.EventPages
{
    public class DeleteModel : PageModel
    {
        private readonly IEventService _eventService;

        [BindProperty]
        public EventDTO EventDTO { get; set; }

        public DeleteModel(IEventService eventService)
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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _eventService.DeleteEventAsync(EventDTO.Id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
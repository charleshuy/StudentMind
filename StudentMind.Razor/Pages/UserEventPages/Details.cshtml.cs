using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentMind.Razor.Pages.UserEventPages
{
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public Event Event { get; set; }

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var eventRepo = _unitOfWork.GetRepository<Event>();
            Event = await eventRepo.Entities
                .Include(e => e.Host)
                .FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);

            if (Event == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEnrollAsync(string eventId)
        {
            // Retrieve the current user's ID from the JWT token
            var jwtToken = HttpContext.Request.Cookies["JWT_Token"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToPage("/Login"); 
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);
            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login"); 
            }

            // Check if the user is already enrolled in the event
            var userEventRepo = _unitOfWork.GetRepository<UserEvent>();
            var existingEnrollment = await userEventRepo.Entities
                .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.ProgramId == eventId);

            if (existingEnrollment != null)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this event.";
                return RedirectToPage(new { id = eventId });
            }

            // Create a new UserEvent to enroll the user
            var userEvent = new UserEvent
            {
                ProgramId = eventId, // Event ID :D dont ask why
                UserId = userId,
                CreatedTime = DateTime.UtcNow
            };

            await userEventRepo.InsertAsync(userEvent);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Successfully enrolled in the event!";
            return RedirectToPage(new { id = eventId });
        }
    }
}
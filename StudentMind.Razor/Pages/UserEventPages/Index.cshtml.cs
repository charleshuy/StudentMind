using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        public async Task<IActionResult> OnPostEnrollAsync(string eventId)
        {
            // Retrieve the current user's ID from the JWT token
            var jwtToken = HttpContext.Request.Cookies["JWT_Token"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToPage("/Login"); // Redirect to login if not authenticated
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);
            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login"); // Redirect to login if user ID is not found
            }

            // Check if the user is already enrolled in the event
            var userEventRepo = _unitOfWork.GetRepository<UserEvent>();
            var existingEnrollment = await userEventRepo.Entities
                .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.ProgramId == eventId);

            if (existingEnrollment != null)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this event.";
                return RedirectToPage();
            }

            // Create a new UserEvent to enroll the user
            var userEvent = new UserEvent
            {
                ProgramId = eventId, // Event ID
                UserId = userId,
                CreatedTime = DateTime.UtcNow
            };

            await userEventRepo.InsertAsync(userEvent);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Successfully enrolled in the event!";
            return RedirectToPage();
        }
    }
}
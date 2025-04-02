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
        public bool IsJoined { get; set; }

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
                return Page();
            }

            // Check if the user has already joined the event
            var userId = GetCurrentUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                var userEventRepo = _unitOfWork.GetRepository<UserEvent>();
                IsJoined = await userEventRepo.Entities
                    .AnyAsync(ue => ue.UserId == userId && ue.ProgramId == id && ue.DeletedTime == null);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            // Check if the user has already joined the event
            var userEventRepo = _unitOfWork.GetRepository<UserEvent>();
            var alreadyJoined = await userEventRepo.Entities
                .AnyAsync(ue => ue.UserId == userId && ue.ProgramId == id && ue.DeletedTime == null);

            if (alreadyJoined)
            {
                return RedirectToPage("Details", new { id });
            }

            // Create a new UserEvent entry
            var userEvent = new UserEvent
            {
                ProgramId = id,
                UserId = userId
            };

            await userEventRepo.InsertAsync(userEvent);
            await _unitOfWork.SaveAsync();

            return RedirectToPage("Details", new { id });
        }

        private string GetCurrentUserId()
        {
            var jwtToken = HttpContext.Request.Cookies["JWT_Token"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);
            return token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
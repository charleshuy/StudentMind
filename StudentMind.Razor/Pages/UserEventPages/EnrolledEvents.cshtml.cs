using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentMind.Razor.Pages.UserEventPages
{
    public class EnrolledEventsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IEnumerable<Event> EnrolledEvents { get; set; }

        public EnrolledEventsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var userEventRepo = _unitOfWork.GetRepository<UserEvent>();
            EnrolledEvents = await userEventRepo.Entities
                .Where(ue => ue.UserId == userId && ue.DeletedTime == null)
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.Host)
                .Select(ue => ue.Event)
                .Where(e => e.DeletedTime == null)
                .ToListAsync();

            return Page();
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
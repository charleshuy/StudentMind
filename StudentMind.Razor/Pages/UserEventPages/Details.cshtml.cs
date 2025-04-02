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
        public IEnumerable<User> JoinedUsers { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        private const int PageSize = 5; 

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync(string id, int page = 1)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the event
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
                IsJoined = await _unitOfWork.GetRepository<UserEvent>().Entities
                    .AnyAsync(ue => ue.UserId == userId && ue.ProgramId == id && ue.DeletedTime == null);
            }

            // Fetch the list of users who joined the event with pagination
            var userEventRepo = _unitOfWork.GetRepository<UserEvent>();
            var joinedUserEvents = userEventRepo.Entities
                .Where(ue => ue.ProgramId == id && ue.DeletedTime == null)
                .Include(ue => ue.User);

            // Calculate pagination
            var totalUsers = await joinedUserEvents.CountAsync();
            TotalPages = (int)Math.Ceiling(totalUsers / (double)PageSize);
            CurrentPage = page < 1 ? 1 : page > TotalPages ? TotalPages : page;

            // Fetch the users for the current page
            JoinedUsers = await joinedUserEvents
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(ue => ue.User)
                .ToListAsync();

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
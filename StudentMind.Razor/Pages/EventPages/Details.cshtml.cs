using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StudentMind.Razor.Pages.EventPages
{
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventDTO EventDTO { get; set; }
        public string DebugMessage { get; set; }

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                DebugMessage = "Event ID is null or empty.";
                return NotFound();
            }

            // Get the logged-in user's ID
            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                DebugMessage = "JWT token missing or invalid.";
                return RedirectToPage("/Login");
            }
            DebugMessage = $"User ID retrieved: {userId}.";

            // Check if the user has the "Psychologist" or "Admin" role
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.Entities
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || (user.Role?.RoleName != "Psychologist" && user.Role?.RoleName != "Admin"))
            {
                DebugMessage += " User does not exist or does not have the 'Psychologist' or 'Admin' role.";
                return RedirectToPage("/Login");
            }

            // Fetch the event
            var eventRepo = _unitOfWork.GetRepository<Event>();
            var eventEntity = await eventRepo.Entities
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null)
            {
                DebugMessage += $" Event with ID {id} not found.";
                return NotFound();
            }

            // Map to EventDTO
            EventDTO = new EventDTO
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                HostId = eventEntity.HostId
            };

            // Fetch the host's name
            var host = await userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == EventDTO.HostId);
            EventDTO.HostName = host?.FullName ?? "Unknown";
            DebugMessage += $" Loaded event with ID {id}, HostName: {EventDTO.HostName}.";

            return Page();
        }

        private string GetUserIdFromToken()
        {
            var jwtToken = Request.Cookies["JWT_Token"];
            if (string.IsNullOrEmpty(jwtToken))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);
            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
    }
}
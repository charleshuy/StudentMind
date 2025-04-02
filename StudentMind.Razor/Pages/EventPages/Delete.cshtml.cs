using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace StudentMind.Razor.Pages.EventPages
{
    [Authorize(AuthenticationSchemes = "Jwt", Roles = "Admin,Psychologist")]
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public EventDTO EventDTO { get; set; }

        public string DebugMessage { get; set; }

        public DeleteModel(IUnitOfWork unitOfWork)
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

            // Get the logged-in user's ID and role
            var (userId, userRole) = await GetUserIdAndRoleAsync();
            if (string.IsNullOrEmpty(userId))
            {
                DebugMessage = "JWT token missing or invalid.";
                return RedirectToPage("/Login");
            }
            DebugMessage = $"User ID retrieved: {userId}, Role: {userRole}.";

            // Check if the user has the "Psychologist" or "Admin" role
            if (userRole != "Psychologist" && userRole != "Admin")
            {
                DebugMessage += " User does not have the 'Psychologist' or 'Admin' role.";
                TempData["ErrorMessage"] = "You have no permission to access this page.";
                return RedirectToPage("/Index");
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

            // Check if the user is the host or an admin
            if (eventEntity.HostId != userId && userRole != "Admin")
            {
                DebugMessage += " User does not have permission to delete this event.";
                TempData["ErrorMessage"] = "You have no permission to delete this event.";
                return RedirectToPage("/EventPages/Index");
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

            // Fetch the host name
            var userRepo = _unitOfWork.GetRepository<User>();
            var host = await userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == EventDTO.HostId);
            EventDTO.HostName = host?.FullName ?? "Unknown";

            DebugMessage += $" Loaded event with ID {id} and host name: {EventDTO.HostName}.";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the logged-in user's ID and role
            var (userId, userRole) = await GetUserIdAndRoleAsync();
            if (string.IsNullOrEmpty(userId))
            {
                DebugMessage = "JWT token missing or invalid.";
                return RedirectToPage("/Login");
            }
            DebugMessage = $"User ID retrieved: {userId}, Role: {userRole}.";

            // Check if the user has the "Psychologist" or "Admin" role
            if (userRole != "Psychologist" && userRole != "Admin")
            {
                DebugMessage += " User does not have the 'Psychologist' or 'Admin' role.";
                TempData["ErrorMessage"] = "You have no permission to access this page.";
                return RedirectToPage("/Index");
            }

            // Fetch the event to validate HostId
            var eventRepo = _unitOfWork.GetRepository<Event>();
            var eventEntity = await eventRepo.Entities
                .FirstOrDefaultAsync(e => e.Id == EventDTO.Id);

            if (eventEntity == null)
            {
                DebugMessage += $" Event with ID {EventDTO.Id} not found.";
                return NotFound();
            }

            // Check if the user is the host or an admin
            if (eventEntity.HostId != userId && userRole != "Admin")
            {
                DebugMessage += " User does not have permission to delete this event.";
                TempData["ErrorMessage"] = "You have no permission to delete this event.";
                return RedirectToPage("/EventPages/Index");
            }

            // Delete the event
            try
            {
                await eventRepo.DeleteAsync(eventEntity);
                await _unitOfWork.SaveAsync();

                DebugMessage += " Event deleted successfully.";
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            catch (Exception ex)
            {
                DebugMessage += $" Error deleting event: {ex.Message}";
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the event. Please try again.");
                return Page();
            }

            return RedirectToPage("Index");
        }

        private async Task<(string userId, string userRole)> GetUserIdAndRoleAsync()
        {
            var jwtToken = Request.Cookies["JWT_Token"];
            if (string.IsNullOrEmpty(jwtToken))
                return (null, null);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);
            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return (null, null);

            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.Entities
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return (userId, user?.Role?.RoleName);
        }
    }
}
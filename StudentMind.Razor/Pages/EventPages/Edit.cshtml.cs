using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StudentMind.Razor.Pages.EventPages
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public EventDTO EventDTO { get; set; }

        public string DebugMessage { get; set; }

        public EditModel(IUnitOfWork unitOfWork)
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

            // Check if the user has permission to edit this event
            if (eventEntity.HostId != userId && user.Role.RoleName != "Admin")
            {
                DebugMessage += " User does not have permission to edit this event.";
                return Forbid();
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

            DebugMessage += $" Loaded event with ID {id}.";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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

            // Fetch the existing event to preserve HostId and check permissions
            var eventRepo = _unitOfWork.GetRepository<Event>();
            var eventEntity = await eventRepo.Entities
                .FirstOrDefaultAsync(e => e.Id == EventDTO.Id);

            if (eventEntity == null)
            {
                DebugMessage += $" Event with ID {EventDTO.Id} not found.";
                return NotFound();
            }

            // Check if the user has permission to edit this event
            if (eventEntity.HostId != userId && user.Role.RoleName != "Admin")
            {
                DebugMessage += " User does not have permission to edit this event.";
                return Forbid();
            }

            //// Validate the model
            //if (!ModelState.IsValid)
            //{
            //    DebugMessage += " Model validation failed.";
            //    return Page();
            //}

            
            if (EventDTO.StartDate.Date < DateTime.Today && EventDTO.StartDate.Date > eventEntity.StartDate.Date)
            {
                DebugMessage += " Start date must be in the future.";
                ModelState.AddModelError("EventDTO.StartDate", "Start date must be today or in the future.");
                return Page();
            }

            // Update the event
            try
            {
                eventEntity.Name = EventDTO.Name;
                eventEntity.Description = EventDTO.Description;
                eventEntity.StartDate = EventDTO.StartDate;
                eventEntity.EndDate = EventDTO.EndDate;
                eventEntity.HostId = eventEntity.HostId; // Preserve the original HostId

                await eventRepo.UpdateAsync(eventEntity);
                await _unitOfWork.SaveAsync();
                DebugMessage += " Event updated successfully.";
                TempData["SuccessMessage"] = "Event updated successfully!";
            }
            catch (Exception ex)
            {
                DebugMessage += $" Error updating event: {ex.Message}";
                ModelState.AddModelError(string.Empty, "An error occurred while updating the event. Please try again.");
                return Page();
            }

            return RedirectToPage("Index");
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
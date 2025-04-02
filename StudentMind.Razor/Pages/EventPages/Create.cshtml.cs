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
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace StudentMind.Razor.Pages.EventPages
{
    [Authorize(AuthenticationSchemes = "Jwt", Roles = "Admin,Psychologist")]
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public EventDTO EventDTO { get; set; }

        public string DebugMessage { get; set; }

        public CreateModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult OnGet()
        {
            EventDTO = new EventDTO();
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
                ModelState.AddModelError(string.Empty, "You do not have permission to create events.");
                return Page();
            }

            // Log the state of EventDTO before validation
            DebugMessage += $" EventDTO before validation: Id={EventDTO.Id}, HostId={EventDTO.HostId}, HostName={EventDTO.HostName}, Name={EventDTO.Name}, StartDate={EventDTO.StartDate}, EndDate={EventDTO.EndDate}.";

            // Set HostId to the logged-in user's ID
            EventDTO.HostId = userId;
            DebugMessage += $" HostId set to: {EventDTO.HostId}.";

            //// Validate the model
            //if (!ModelState.IsValid)
            //{
            //    // Log specific validation errors
            //    var errors = ModelState
            //        .Where(x => x.Value.Errors.Count > 0)
            //        .Select(x => new { x.Key, x.Value.Errors })
            //        .ToList();

            //    DebugMessage += " Model validation failed. Errors: ";
            //    foreach (var error in errors)
            //    {
            //        foreach (var err in error.Errors)
            //        {
            //            DebugMessage += $"{error.Key}: {err.ErrorMessage}; ";
            //        }
            //    }

            //    // Log the state of EventDTO after validation failure
            //    DebugMessage += $" EventDTO after validation failure: Id={EventDTO.Id}, HostId={EventDTO.HostId}, HostName={EventDTO.HostName}.";
            //    return Page();
            //}
            //validate make this page error lol, i might figure it out later .-.

            
            if (EventDTO.StartDate.Date < DateTime.Today)
            {
                DebugMessage += " Start date must be in the future.";
                ModelState.AddModelError("EventDTO.StartDate", "Start date must be today or in the future.");
                return Page();
            }

            
            try
            {
                var eventEntity = new Event
                {
                    Id = Guid.NewGuid().ToString(), // Generate a new ID
                    Name = EventDTO.Name,
                    Description = EventDTO.Description,
                    StartDate = EventDTO.StartDate,
                    EndDate = EventDTO.EndDate,
                    HostId = EventDTO.HostId
                };

                var eventRepo = _unitOfWork.GetRepository<Event>();
                await eventRepo.InsertAsync(eventEntity);
                await _unitOfWork.SaveAsync();

                DebugMessage += " Event created successfully.";
                TempData["SuccessMessage"] = "Event created successfully!";
            }
            catch (Exception ex)
            {
                DebugMessage += $" Error creating event: {ex.Message}";
                ModelState.AddModelError(string.Empty, "An error occurred while creating the event. Please try again.");
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
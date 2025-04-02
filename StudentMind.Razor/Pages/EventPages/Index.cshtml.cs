using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StudentMind.Razor.Pages.EventPages
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IEnumerable<EventDTO> Events { get; set; }
        public string DebugMessage { get; set; }

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync()
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
                return RedirectToPage("/Index");
            }

            // Fetch all events
            var eventRepo = _unitOfWork.GetRepository<Event>();
            var events = await eventRepo.Entities
                .Where(e => e.StartDate >= DateTime.Today) 
                .OrderBy(e => e.StartDate) 
                .ToListAsync();

            DebugMessage += $" Found {events.Count} events.";

            
            Events = events.Select(e => new EventDTO
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                HostId = e.HostId
            }).ToList();

            
            foreach (var evt in Events)
            {
                var host = await userRepo.Entities
                    .FirstOrDefaultAsync(u => u.Id == evt.HostId);
                evt.HostName = host?.FullName ?? "Unknown";
            }

            DebugMessage += $" Loaded host names for {Events.Count()} events.";

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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Core.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StudentMind.Razor.Pages.AppointmentPages
{
    public class PsychologistAppointmentsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StudentNameFilter { get; set; }

        // Sorting properties
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool SortDescending { get; set; }

        public List<Appointment> Appointments { get; set; }
        public SelectList StatusList { get; set; }
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public string DebugMessage { get; set; }

        public PsychologistAppointmentsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Unable to identify the current user. Please log in again.");
            }
            
            var psychologistId = userId;

            // Validate that the psychologist exists and has the correct role
            var userRepo = _unitOfWork.GetRepository<User>();
            var psychologist = await userRepo.Entities
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == psychologistId && u.Role != null && u.Role.RoleName == "Psychologist");

            if (psychologist == null)
            {
                DebugMessage = "Psychologist account not found or invalid.";
                return Page();
            }

            // Populate the status dropdown
            var statusOptions = Enum.GetValues(typeof(EnumStatus))
                .Cast<EnumStatus>()
                .Select(s => new { Value = s.ToString(), Text = s.ToString() })
                .ToList();
            statusOptions.Insert(0, new { Value = "", Text = "All" });
            StatusList = new SelectList(statusOptions, "Value", "Text", StatusFilter);

            // Query appointments for the psychologist
            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var query = appointmentRepo.Entities
                .Include(a => a.User) // Include the student (User) for name display
                .Where(a => a.PsychologistId == psychologistId);

            // Apply filters
            if (FromDate.HasValue)
            {
                query = query.Where(a => a.StartTime.Date >= FromDate.Value);
            }

            if (ToDate.HasValue)
            {
                query = query.Where(a => a.StartTime.Date <= ToDate.Value);
            }

            if (!string.IsNullOrEmpty(StatusFilter) && Enum.TryParse<EnumStatus>(StatusFilter, out var status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (!string.IsNullOrEmpty(StudentNameFilter))
            {
                query = query.Where(a => a.User != null && a.User.FullName.Contains(StudentNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Apply sorting
            if (string.IsNullOrEmpty(SortBy))
            {
                SortBy = "StartTime"; // Default sort by StartTime
                SortDescending = false;
            }

            switch (SortBy.ToLower())
            {
                case "studentname":
                    query = SortDescending
                        ? query.OrderByDescending(a => a.User != null ? a.User.FullName : "")
                        : query.OrderBy(a => a.User != null ? a.User.FullName : "");
                    break;
                case "status":
                    query = SortDescending
                        ? query.OrderByDescending(a => a.Status)
                        : query.OrderBy(a => a.Status);
                    break;
                case "starttime":
                default:
                    query = SortDescending
                        ? query.OrderByDescending(a => a.StartTime)
                        : query.OrderBy(a => a.StartTime);
                    break;
            }

            // Execute the query
            Appointments = await query.ToListAsync();

            // Calculate summary statistics
            TotalAppointments = Appointments.Count;
            PendingAppointments = Appointments.Count(a => a.Status == EnumStatus.Pending);

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {

            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Unable to identify the current user. Please log in again.");
            }
            else
            {
                // Validate that the user exists and has the "Psychologist" role
                var userRepo = _unitOfWork.GetRepository<User>();
                var currentUser = await userRepo.Entities
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.Role != null && u.Role.RoleName == "Psychologist");

                if (currentUser == null)
                {
                    ModelState.AddModelError("", "Current user does not exist or is not a valid student.");
                }
            }

            var psychologistId = userId;

            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var appointment = await appointmentRepo.Entities
                .FirstOrDefaultAsync(a => a.Id.Equals(id) && a.PsychologistId == psychologistId);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found or you do not have permission to modify it.";
                return RedirectToPage();
            }

            if (appointment.Status != EnumStatus.Pending)
            {
                TempData["ErrorMessage"] = "Only pending appointments can be approved.";
                return RedirectToPage();
            }

            appointment.Status = EnumStatus.Approved;
            await appointmentRepo.UpdateAsync(appointment);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Appointment approved successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Unable to identify the current user. Please log in again.");
            }
            else
            {
                // Validate that the user exists and has the "Psychologist" role
                var userRepo = _unitOfWork.GetRepository<User>();
                var currentUser = await userRepo.Entities
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.Role != null && u.Role.RoleName == "Psychologist");

                if (currentUser == null)
                {
                    ModelState.AddModelError("", "Current user does not exist or is not a valid student.");
                }
            }

            var psychologistId = userId;

            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var appointment = await appointmentRepo.Entities
                .FirstOrDefaultAsync(a => a.Id.Equals(id) && a.PsychologistId == psychologistId);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found or you do not have permission to modify it.";
                return RedirectToPage();
            }

            if (appointment.Status == EnumStatus.Cancelled)
            {
                TempData["ErrorMessage"] = "Appointment is already cancelled.";
                return RedirectToPage();
            }

            appointment.Status = EnumStatus.Cancelled;
            await appointmentRepo.UpdateAsync(appointment);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            return RedirectToPage();
        }

        public string GetUserIdFromToken()
        {
            var jwtToken = HttpContext.Session.GetString("JWT_Token");

            if (string.IsNullOrEmpty(jwtToken))
                throw new Exception("No JWT token found in session.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);

            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new Exception("NameIdentifier (User ID) claim not found in token.");

            return userId;
        }
    }
}
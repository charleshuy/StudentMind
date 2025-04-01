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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentMind.Razor.Pages.AppointmentPages
{
    public class StudentAppointmentsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PsychologistNameFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool SortDescending { get; set; }

        public List<Appointment> Appointments { get; set; }
        public SelectList StatusList { get; set; }
        public int TotalAppointments { get; set; }
        public string DebugMessage { get; set; }

        public StudentAppointmentsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var studentId = userId;

            var userRepo = _unitOfWork.GetRepository<User>();
            var student = await userRepo.Entities
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == studentId && u.Role != null && u.Role.RoleName == "User");

            if (student == null)
            {
                DebugMessage = "Student account not found or invalid.";
                return Page();
            }

            var statusOptions = Enum.GetValues(typeof(EnumStatus))
                .Cast<EnumStatus>()
                .Select(s => new { Value = s.ToString(), Text = s.ToString() })
                .ToList();
            statusOptions.Insert(0, new { Value = "", Text = "All" });
            StatusList = new SelectList(statusOptions, "Value", "Text", StatusFilter);

            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var query = appointmentRepo.Entities
                .Include(a => a.Psychologist)
                .Where(a => a.UserId == studentId);

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

            if (!string.IsNullOrEmpty(PsychologistNameFilter))
            {
                query = query.Where(a => a.Psychologist != null && a.Psychologist.FullName.Contains(PsychologistNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (string.IsNullOrEmpty(SortBy))
            {
                SortBy = "StartTime";
                SortDescending = false;
            }

            switch (SortBy.ToLower())
            {
                case "psychologistname":
                    query = SortDescending
                        ? query.OrderByDescending(a => a.Psychologist != null ? a.Psychologist.FullName : "")
                        : query.OrderBy(a => a.Psychologist != null ? a.Psychologist.FullName : "");
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

            Appointments = await query.ToListAsync();

            TotalAppointments = Appointments.Count;

            return Page();
        }

        public string GetUserIdFromToken()
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
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
using Microsoft.AspNetCore.Authorization;

namespace StudentMind.Razor.Pages.AppointmentPages
{
    public class BookModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

        [BindProperty(SupportsGet = true)]
        public string SelectedPsychologistId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime SelectedDate { get; set; }

        [BindProperty]
        public string SelectedSlot { get; set; }

        public SelectList PsychologistList { get; set; }
        public User SelectedPsychologist { get; set; }
        public List<(DateTimeOffset Start, DateTimeOffset End, bool IsAvailable)> AvailableSlots { get; set; }
        public string DebugMessage { get; set; }

        public BookModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load psychologists for the dropdown
            var psychologistRepo = _unitOfWork.GetRepository<User>();
            var psychologists = await psychologistRepo.Entities
                .Include(u => u.Role)
                .Where(u => u.Role != null && u.Role.RoleName == "Psychologist")
                .ToListAsync();

            if (!psychologists.Any())
            {
                DebugMessage = "No psychologists are available at this time.";
            }
            else
            {
                DebugMessage = $"Found {psychologists.Count} psychologists.";
            }

            PsychologistList = new SelectList(psychologists, "Id", "FullName", SelectedPsychologistId);

            // If a psychologist is selected, load their details
            if (!string.IsNullOrEmpty(SelectedPsychologistId))
            {
                DebugMessage += $" SelectedPsychologistId: {SelectedPsychologistId}.";
                SelectedPsychologist = await psychologistRepo.Entities
                    .FirstOrDefaultAsync(u => u.Id == SelectedPsychologistId && u.Role != null && u.Role.RoleName == "Psychologist");

                if (SelectedPsychologist == null)
                {
                    DebugMessage += " Selected psychologist not found or not a valid psychologist.";
                }
                else
                {
                    DebugMessage += $" Selected psychologist: {SelectedPsychologist.FullName}.";
                }
            }

            // Set a default date if none is selected
            if (SelectedDate == default && SelectedPsychologist != null)
            {
                SelectedDate = DateTime.Today;
            }

            // If a date is selected and a psychologist is selected, calculate available slots
            if (SelectedDate != default && SelectedPsychologist != null)
            {
                DebugMessage += $" Calculating slots for date: {SelectedDate.ToString("yyyy-MM-dd")}.";
                AvailableSlots = await CalculateAvailableSlots(SelectedPsychologist.Id, SelectedDate);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Repopulate dropdown and psychologist info in case of validation failure
            var psychologistRepo = _unitOfWork.GetRepository<User>();
            var psychologists = await psychologistRepo.Entities
                .Include(u => u.Role)
                .Where(u => u.Role != null && u.Role.RoleName == "Psychologist")
                .ToListAsync();

            if (!psychologists.Any())
            {
                DebugMessage = "No psychologists are available at this time.";
            }

            PsychologistList = new SelectList(psychologists, "Id", "FullName", SelectedPsychologistId);

            SelectedPsychologist = await psychologistRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == SelectedPsychologistId && u.Role != null && u.Role.RoleName == "Psychologist");

            if (SelectedPsychologist == null)
            {
                ModelState.AddModelError("SelectedPsychologistId", "Selected psychologist does not exist or is not a valid psychologist.");
            }

            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Unable to identify the current user. Please log in again.");
                return Page();
            }

            // Validate that the user exists and has the "User" role
            var userRepo = _unitOfWork.GetRepository<User>();
            var currentUser = await userRepo.Entities
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId && u.Role != null && u.Role.RoleName == "User");

            if (currentUser == null)
            {
                ModelState.AddModelError("", "The user does not exist or is not a valid student.");
            }

            // Validate the selected date
            if (SelectedDate < DateTime.Today)
            {
                ModelState.AddModelError("SelectedDate", "Selected date must be today or in the future.");
            }

            // Validate the selected slot and set StartTime and EndTime
            if (string.IsNullOrEmpty(SelectedSlot))
            {
                ModelState.AddModelError("SelectedSlot", "Please select a time slot.");
            }
            else
            {
                var slotTimes = GetSlotTimes(SelectedDate);
                var selectedSlotTimes = slotTimes.FirstOrDefault(s => $"{s.Start:HH:mm}-{s.End:HH:mm}" == SelectedSlot);

                if (selectedSlotTimes.Start == default || selectedSlotTimes.End == default)
                {
                    ModelState.AddModelError("SelectedSlot", "Invalid time slot selected.");
                }
                else
                {
                    Appointment.StartTime = selectedSlotTimes.Start;
                    Appointment.EndTime = selectedSlotTimes.End;
                }
            }

            // Validate no overlapping appointments
            if (ModelState.IsValid)
            {
                var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
                var overlappingAppointments = await appointmentRepo.Entities
                    .Where(a => a.PsychologistId == SelectedPsychologistId &&
                                a.Status != EnumStatus.Cancelled &&
                                ((Appointment.StartTime >= a.StartTime && Appointment.StartTime < a.EndTime) ||
                                 (Appointment.EndTime > a.StartTime && Appointment.EndTime <= a.EndTime) ||
                                 (Appointment.StartTime <= a.StartTime && Appointment.EndTime >= a.EndTime)))
                    .AnyAsync();

                if (overlappingAppointments)
                {
                    ModelState.AddModelError("SelectedSlot", "The selected time slot is no longer available.");
                }
            }

            if (!ModelState.IsValid)
            {
                AvailableSlots = await CalculateAvailableSlots(SelectedPsychologistId, SelectedDate);
                return Page();
            }

            // Set appointment details
            Appointment.PsychologistId = SelectedPsychologistId;
            Appointment.UserId = userId;
            Appointment.Status = EnumStatus.Pending;

            // Save the appointment
            var appointmentRepoSave = _unitOfWork.GetRepository<Appointment>();
            await appointmentRepoSave.InsertAsync(Appointment);
            await _unitOfWork.SaveAsync();

            return RedirectToPage("./Index");
        }

        private async Task<List<(DateTimeOffset Start, DateTimeOffset End, bool IsAvailable)>> CalculateAvailableSlots(string psychologistId, DateTime date)
        {
            var slots = GetSlotTimes(date);
            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var appointments = await appointmentRepo.Entities
                .Where(a => a.PsychologistId == psychologistId &&
                            a.StartTime.Date == date.Date &&
                            a.Status != EnumStatus.Cancelled)
                .ToListAsync();

            var availableSlots = new List<(DateTimeOffset Start, DateTimeOffset End, bool IsAvailable)>();

            foreach (var slot in slots)
            {
                bool isAvailable = !appointments.Any(a =>
                    (slot.Start >= a.StartTime && slot.Start < a.EndTime) ||
                    (slot.End > a.StartTime && slot.End <= a.EndTime) ||
                    (slot.Start <= a.StartTime && slot.End >= a.EndTime));

                availableSlots.Add((slot.Start, slot.End, isAvailable));
            }

            return availableSlots;
        }

        private List<(DateTimeOffset Start, DateTimeOffset End)> GetSlotTimes(DateTime date)
        {
            var slots = new List<(DateTimeOffset Start, DateTimeOffset End)>();
            // Use DateTimeKind.Unspecified to avoid automatic offset assumptions
            var baseDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);
            // Define the offset for UTC+7
            var offset = TimeSpan.FromHours(7);

            slots.Add((
                new DateTimeOffset(baseDate.AddHours(7).AddMinutes(30), offset),
                new DateTimeOffset(baseDate.AddHours(9).AddMinutes(30), offset)
            ));
            slots.Add((
                new DateTimeOffset(baseDate.AddHours(10), offset),
                new DateTimeOffset(baseDate.AddHours(12), offset)
            ));
            slots.Add((
                new DateTimeOffset(baseDate.AddHours(14), offset),
                new DateTimeOffset(baseDate.AddHours(16), offset)
            ));
            slots.Add((
                new DateTimeOffset(baseDate.AddHours(16).AddMinutes(30), offset),
                new DateTimeOffset(baseDate.AddHours(18).AddMinutes(30), offset)
            ));

            return slots;
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
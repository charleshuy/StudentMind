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
        public List<SelectListItem> AvailableSlots { get; set; } = new List<SelectListItem>();
        public string DebugMessage { get; set; }
        public User SelectedPsychologist { get; set; }

        public BookModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateDropdownsAsync();

            // Set default date to today if not selected
            if (SelectedDate == default)
            {
                SelectedDate = DateTime.Today;
                DebugMessage = "Default date set to today.";
            }
            else
            {
                DebugMessage = $"Selected date: {SelectedDate.ToString("yyyy-MM-dd")}";
            }

            // Set Appointment.PsychologistId from the query parameter and load psychologist details
            if (!string.IsNullOrEmpty(SelectedPsychologistId))
            {
                Appointment.PsychologistId = SelectedPsychologistId;
                DebugMessage += $" Psychologist selected: {SelectedPsychologistId}.";

                // Load the selected psychologist's details
                var psychologistRepo = _unitOfWork.GetRepository<User>();
                SelectedPsychologist = await psychologistRepo.Entities
                    .FirstOrDefaultAsync(u => u.Id == SelectedPsychologistId);

                if (SelectedPsychologist == null)
                {
                    DebugMessage += " Selected psychologist not found.";
                }
                else
                {
                    DebugMessage += $" Loaded psychologist: {SelectedPsychologist.FullName}.";
                }

                await PopulateAvailableSlotsAsync(SelectedPsychologistId, SelectedDate);
                DebugMessage += $" Found {AvailableSlots.Count} available slots.";
            }
            else
            {
                DebugMessage += " No psychologist selected.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await PopulateDropdownsAsync();

            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                DebugMessage = "JWT token missing or invalid.";
                return RedirectToPage("/Account/Login");
            }
            DebugMessage = $"User ID retrieved: {userId}.";

            // Set Appointment.PsychologistId from SelectedPsychologistId
            Appointment.PsychologistId = SelectedPsychologistId;
            DebugMessage += $" Psychologist ID set: {SelectedPsychologistId}.";

            // Load the selected psychologist's details for display after form submission
            if (!string.IsNullOrEmpty(SelectedPsychologistId))
            {
                var psychologistRepo = _unitOfWork.GetRepository<User>();
                SelectedPsychologist = await psychologistRepo.Entities
                    .FirstOrDefaultAsync(u => u.Id == SelectedPsychologistId);
            }

            // Populate available slots (needed to set StartTime and EndTime)
            if (!string.IsNullOrEmpty(Appointment.PsychologistId))
            {
                await PopulateAvailableSlotsAsync(Appointment.PsychologistId, SelectedDate);
            }

            // Set StartTime and EndTime from the selected slot
            if (!string.IsNullOrEmpty(SelectedSlot))
            {
                var slotTimes = GetSlotTimes(SelectedDate);
                var selectedSlotTimes = slotTimes.FirstOrDefault(s => $"{s.Start:HH:mm}-{s.End:HH:mm}" == SelectedSlot);
                if (selectedSlotTimes.Start != default && selectedSlotTimes.End != default)
                {
                    Appointment.StartTime = selectedSlotTimes.Start;
                    Appointment.EndTime = selectedSlotTimes.End;
                    DebugMessage += $" Slot set: {SelectedSlot}, StartTime: {Appointment.StartTime}, EndTime: {Appointment.EndTime}.";
                }
                else
                {
                    DebugMessage += " Invalid time slot selected.";
                }
            }
            else
            {
                DebugMessage += " No time slot selected.";
            }

            // Set appointment details
            Appointment.UserId = userId;
            Appointment.Status = EnumStatus.Pending;

            // Save the appointment
            try
            {
                var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
                await appointmentRepo.InsertAsync(Appointment);
                await _unitOfWork.SaveAsync();
                DebugMessage += " Appointment saved successfully.";

                // Set success message in TempData
                TempData["SuccessMessage"] = "Appointment booked successfully!";
            }
            catch (Exception ex)
            {
                DebugMessage += $" Error saving appointment: {ex.Message}";
                return Page();
            }

            // Redirect to StudentAppointments page
            return RedirectToPage("./StudentAppointments");
        }

        private async Task PopulateDropdownsAsync()
        {
            var psychologistRepo = _unitOfWork.GetRepository<User>();
            var psychologists = await psychologistRepo.Entities
                .Include(u => u.Role)
                .Where(u => u.Role != null && u.Role.RoleName == "Psychologist")
                .ToListAsync();

            PsychologistList = new SelectList(psychologists, "Id", "FullName", SelectedPsychologistId);
        }

        private async Task PopulateAvailableSlotsAsync(string psychologistId, DateTime date)
        {
            var slots = GetSlotTimes(date);
            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var appointments = await appointmentRepo.Entities
                .Where(a => a.PsychologistId == psychologistId &&
                            a.StartTime.Date == date.Date &&
                            a.Status != EnumStatus.Cancelled)
                .ToListAsync();

            AvailableSlots = new List<SelectListItem>();
            foreach (var slot in slots)
            {
                bool isAvailable = !appointments.Any(a =>
                    (slot.Start >= a.StartTime && slot.Start < a.EndTime) ||
                    (slot.End > a.StartTime && slot.End <= a.EndTime) ||
                    (slot.Start <= a.StartTime && slot.End >= a.EndTime));

                if (isAvailable)
                {
                    var slotValue = $"{slot.Start:HH:mm}-{slot.End:HH:mm}";
                    AvailableSlots.Add(new SelectListItem
                    {
                        Value = slotValue,
                        Text = $"{slot.Start:HH:mm} - {slot.End:HH:mm}"
                    });
                }
            }
        }

        private List<(DateTimeOffset Start, DateTimeOffset End)> GetSlotTimes(DateTime date)
        {
            var slots = new List<(DateTimeOffset Start, DateTimeOffset End)>();
            var baseDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);
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
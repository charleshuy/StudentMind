using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Core.Enum;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace StudentMind.Razor.Pages.AppointmentPages
{
    [Authorize(AuthenticationSchemes = "Jwt", Roles ="Admin,Psychologist")]
    class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

        public SelectList PsychologistList { get; set; }
        public SelectList StudentList { get; set; }

        public CreateModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await PopulateDropdownsAsync();

            Appointment.Status = EnumStatus.Pending;

            var psychologistRepo = _unitOfWork.GetRepository<User>();
            var psychologistExists = await psychologistRepo.ExistsAsync(u => u.Id == Appointment.PsychologistId && u.Role != null && u.Role.RoleName == "Psychologist");
            if (!psychologistExists)
            {
                ModelState.AddModelError("Appointment.PsychologistId", "Selected psychologist does not exist or is not a valid psychologist.");
            }

            var studentExists = await psychologistRepo.ExistsAsync(u => u.Id == Appointment.UserId && u.Role != null && u.Role.RoleName == "User");
            if (!studentExists)
            {
                ModelState.AddModelError("Appointment.UserId", "Selected student does not exist or is not a valid student.");
            }

            if (Appointment.StartTime < DateTimeOffset.UtcNow)
            {
                ModelState.AddModelError("Appointment.StartTime", "Start time must be in the future.");
            }

            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var overlappingAppointments = await appointmentRepo.Entities
                .Where(a => a.PsychologistId == Appointment.PsychologistId &&
                            a.Id != Appointment.Id && 
                            a.Status != EnumStatus.Cancelled &&
                            ((Appointment.StartTime >= a.StartTime && Appointment.StartTime < a.EndTime) ||
                             (Appointment.EndTime > a.StartTime && Appointment.EndTime <= a.EndTime) ||
                             (Appointment.StartTime <= a.StartTime && Appointment.EndTime >= a.EndTime)))
                .AnyAsync();

            if (overlappingAppointments)
            {
                ModelState.AddModelError("Appointment.StartTime", "The selected time slot overlaps with an existing appointment for this psychologist.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await appointmentRepo.InsertAsync(Appointment);
            await _unitOfWork.SaveAsync();

            return RedirectToPage("./Index");
        }

        private async Task PopulateDropdownsAsync()
        {
            var psychologistRepo = _unitOfWork.GetRepository<User>();
            var studentRepo = _unitOfWork.GetRepository<User>();

            var psychologists = await psychologistRepo.Entities
                .Include(u => u.Role)
                .Where(u => u.Role != null && u.Role.RoleName == "Psychologist")
                .ToListAsync();

            var students = await studentRepo.Entities
                .Include(u => u.Role)
                .Where(u => u.Role != null && u.Role.RoleName == "User")
                .ToListAsync();

            PsychologistList = new SelectList(psychologists, "Id", "FullName");
            StudentList = new SelectList(students, "Id", "Username");
        }
    }
}
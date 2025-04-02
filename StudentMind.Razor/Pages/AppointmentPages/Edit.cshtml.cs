using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Core.Enum;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StudentMind.Razor.Pages.AppointmentPages
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            Appointment = await appointmentRepo.Entities
                .Include(a => a.Psychologist)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Appointment == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate StartTime
            if (Appointment.StartTime < DateTimeOffset.UtcNow)
            {
                ModelState.AddModelError("Appointment.StartTime", "Start time must be in the future.");
            }

            // Check for overlapping appointments
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
                Appointment.Psychologist = await _unitOfWork.GetRepository<User>().GetByIdAsync(Appointment.PsychologistId);
                Appointment.User = await _unitOfWork.GetRepository<User>().GetByIdAsync(Appointment.UserId);
                return Page();
            }

            try
            {
                // Fetch the original appointment to preserve the Status
                var originalAppointment = await appointmentRepo.GetByIdAsync(Appointment.Id);
                if (originalAppointment == null)
                {
                    return NotFound();
                }

                // Update the fields we allow to be edited
                originalAppointment.StartTime = Appointment.StartTime;
                originalAppointment.EndTime = Appointment.EndTime;
                originalAppointment.Note = Appointment.Note;
                // Status is not updated, so it remains unchanged

                appointmentRepo.Update(originalAppointment);
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await appointmentRepo.ExistsAsync(a => a.Id == Appointment.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
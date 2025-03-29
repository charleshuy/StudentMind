using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StudentMind.Razor.Pages.AppointmentPages
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Appointment Appointment { get; set; }

        public DeleteModel(IUnitOfWork unitOfWork)
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var appointment = await appointmentRepo.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointmentRepo.Delete(appointment);
            await _unitOfWork.SaveAsync();

            return RedirectToPage("./Index");
        }
    }
}
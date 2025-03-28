using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StudentMind.Razor.Pages.AppointmentPages
{
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public Appointment Appointment { get; set; }

        public DetailsModel(IUnitOfWork unitOfWork)
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
    }
}
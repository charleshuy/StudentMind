using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentMind.Razor.Pages.AppointmentPages
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IList<Appointment> Appointments { get; set; }

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task OnGetAsync()
        {
            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            Appointments = await appointmentRepo.Entities
                .Include(a => a.Psychologist)
                .Include(a => a.User)
                .ToListAsync();
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentMind.Core.Entity;

namespace StudentMind.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(string id);
        Task<Appointment?> GetAppointmentByIdAsync(string id);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetAppointmentsByUserAsync(string userId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPsychologistAsync(string psychologistId);
    }
}
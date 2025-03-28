using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Core.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentMind.Services.Interfaces;

namespace StudentMind.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            var repository = _unitOfWork.GetRepository<Appointment>();

            // Optional: Add any specific validation
            await repository.InsertAsync(appointment);
            await _unitOfWork.SaveAsync();

            return appointment;
        }

        public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
        {
            var repository = _unitOfWork.GetRepository<Appointment>();

            repository.Update(appointment);
            await _unitOfWork.SaveAsync();

            return appointment;
        }

        public async Task<bool> DeleteAppointmentAsync(string id)
        {
            var repository = _unitOfWork.GetRepository<Appointment>();
            var appointment = await repository.GetByIdAsync(id);

            if (appointment == null) return false;

            repository.Delete(appointment);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(string id)
        {
            var repository = _unitOfWork.GetRepository<Appointment>();

            // Note: You might need to configure eager loading in your DbContext
            return await repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            var repository = _unitOfWork.GetRepository<Appointment>();
            return await repository.GetAllAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserAsync(string userId)
        {
            var repository = _unitOfWork.GetRepository<Appointment>();

            return await repository.Entities
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPsychologistAsync(string psychologistId)
        {
            var repository = _unitOfWork.GetRepository<Appointment>();

            return await repository.Entities
                .Where(a => a.PsychologistId == psychologistId)
                .ToListAsync();
        }
    }
}
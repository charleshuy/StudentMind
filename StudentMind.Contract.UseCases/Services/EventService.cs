using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Services.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EventDTO?> GetEventByIdAsync(string eventId)
        {
            var eventRepo = _unitOfWork.GetRepository<Event>();
            var eventEntity = await eventRepo.GetByIdAsync(eventId);

            if (eventEntity == null)
                return null;

            var eventDTO = MapToEventDTO(eventEntity);
            return eventDTO;
        }

        public async Task<IEnumerable<EventDTO>> GetAllEventsAsync()
        {
            var eventRepo = _unitOfWork.GetRepository<Event>();
            var events = await eventRepo.GetAllAsync();

            var eventDTOs = events.Select(MapToEventDTO).ToList();
            return eventDTOs;
        }

        public async Task CreateEventAsync(EventDTO eventDTO)
        {
            var eventRepo = _unitOfWork.GetRepository<Event>();

            var eventEntity = new Event
            {
                Name = eventDTO.Name,
                Description = eventDTO.Description,
                StartDate = eventDTO.StartDate,
                EndDate = eventDTO.EndDate,
                HostId = eventDTO.HostId
            };

            await eventRepo.InsertAsync(eventEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateEventAsync(string eventId, EventDTO eventDTO)
        {
            var eventRepo = _unitOfWork.GetRepository<Event>();
            var eventEntity = await eventRepo.GetByIdAsync(eventId);

            if (eventEntity == null)
                throw new Exception("Event not found.");

            eventEntity.Name = eventDTO.Name;
            eventEntity.Description = eventDTO.Description;
            eventEntity.StartDate = eventDTO.StartDate;
            eventEntity.EndDate = eventDTO.EndDate;
            eventEntity.HostId = eventDTO.HostId;

            eventRepo.Update(eventEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteEventAsync(string eventId)
        {
            var eventRepo = _unitOfWork.GetRepository<Event>();
            var eventEntity = await eventRepo.GetByIdAsync(eventId);

            if (eventEntity == null)
                throw new Exception("Event not found.");

            eventRepo.Delete(eventEntity);
            await _unitOfWork.SaveAsync();
        }

        private EventDTO MapToEventDTO(Event eventEntity)
        {
            return new EventDTO
            {
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                HostId = eventEntity.HostId,
                HostName = eventEntity.Host?.Username 
            };
        }
    }
}

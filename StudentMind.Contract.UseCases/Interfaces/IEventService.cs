

using StudentMind.Services.DTO;

namespace StudentMind.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventDTO?> GetEventByIdAsync(string eventId);
        Task<IEnumerable<EventDTO>> GetAllEventsAsync();
        Task CreateEventAsync(EventDTO eventDTO);
        Task UpdateEventAsync(string eventId, EventDTO eventDTO);
        Task DeleteEventAsync(string eventId);
    }
}

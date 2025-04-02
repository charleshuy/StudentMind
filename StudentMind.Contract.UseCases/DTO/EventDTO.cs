namespace StudentMind.Services.DTO
{
    public class EventDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? HostId { get; set; }

        public string? HostName { get; set; }
    }
}

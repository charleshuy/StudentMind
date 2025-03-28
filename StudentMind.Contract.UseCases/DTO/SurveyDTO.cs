namespace StudentMind.Services.DTO
{
    public class SurveyDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalParticipants { get; set; }
        public string? TypeId { get; set; }
    }
}

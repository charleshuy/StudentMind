using System.ComponentModel.DataAnnotations;

public class EventDTO
{
    public string Id { get; set; }
    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(100, ErrorMessage = "Event name cannot be longer than 100 characters.")]
    public string Name { get; set; }
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(EventDTO), nameof(ValidateEndDate))]
    public DateTime EndDate { get; set; }
    public string HostId { get; set; }
    public string HostName { get; set; }
    public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
    {
        var instance = (EventDTO)context.ObjectInstance;
        if (endDate < instance.StartDate)
        {
            return new ValidationResult("End date must be on or after the start date.");
        }
        return ValidationResult.Success;
    }
}
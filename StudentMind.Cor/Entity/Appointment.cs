using StudentMind.Core.Base;
using StudentMind.Core.Entity;
using StudentMind.Core.Enum;
using System.ComponentModel.DataAnnotations;

public class Appointment : BaseEntity
{
    [Required(ErrorMessage = "Psychologist ID is required.")]
    public string PsychologistId { get; set; } = string.Empty; // Ensure it has a default value

    [Required(ErrorMessage = "User ID is required.")]
    public string UserId { get; set; } = string.Empty; // Ensure it has a default value

    [Required]
    public DateTimeOffset StartTime { get; set; }

    [Required]
    //[DateGreaterThan("StartTime", ErrorMessage = "EndTime must be after StartTime.")]
    public DateTimeOffset EndTime { get; set; }

    public string? Note { get; set; }

    public EnumStatus Status { get; set; }

    // Navigation properties (Optional, but ensure they are nullable to avoid issues)
    public User? Psychologist { get; set; }
    public User? User { get; set; }
}

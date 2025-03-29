using StudentMind.Core.Base;
using StudentMind.Core.Entity;
using StudentMind.Core.Enum;
using System.ComponentModel.DataAnnotations;
using StudentMind.Core.Validations;

namespace StudentMind.Core.Entity
{
    public class Appointment : BaseEntity
    {
        [Required(ErrorMessage = "Psychologist is required.")]
        public string PsychologistId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start time is required.")]
        [FutureDate]
        public DateTimeOffset StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [DateGreaterThan("StartTime", ErrorMessage = "End time must be after start time.")]
        public DateTimeOffset EndTime { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid status.")]
        public EnumStatus Status { get; set; } = EnumStatus.Pending;

        public User? Psychologist { get; set; }
        public User? User { get; set; }
    }
}
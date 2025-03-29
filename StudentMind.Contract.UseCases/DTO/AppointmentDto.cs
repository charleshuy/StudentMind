using StudentMind.Core.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace StudentMind.Services.DTO
{
    public class AppointmentDto
    {
        [Required(ErrorMessage = "Psychologist is required")]
        [Display(Name = "Psychologist")]
        public string PsychologistId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTimeOffset EndTime { get; set; }

        [Display(Name = "Additional Notes")]
        public string? Note { get; set; }

        [Display(Name = "Status")]
        public EnumStatus Status { get; set; }
    }
}
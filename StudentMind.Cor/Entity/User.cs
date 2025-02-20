﻿using StudentMind.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace StudentMind.Core.Entity
{
    public class User : BaseEntity
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string Gender { get; set; } = "Male"; // Male, Female, Others
        public string? Email { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public string? VerificationToken { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTimeOffset? ResetTokenExpiry { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Status { get; set; } = true;

        // Navigation Properties
        public virtual Role? Role { get; set; }
        public virtual ICollection<Certificate>? Certificates { get; set; } = new List<Certificate>();
        public virtual ICollection<Appointment>? PsychologistAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Appointment>? UserAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<SurveyResponse>? SurveyResponses { get; set; } = new List<SurveyResponse>();
        public virtual ICollection<UserEvent>? UserEvents { get; set; } = new List<UserEvent>();
        public virtual ICollection<History>? Histories { get; set; } = new List<History>();

    }
}

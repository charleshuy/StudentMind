using System.ComponentModel.DataAnnotations.Schema;
using StudentMind.Core.Base;

namespace StudentMind.Core.Entity
{
    public class Appointment : BaseEntity
    {
        public string PsychologistId { get; set; }

        public string UserId { get; set; }

        public User Psychologist { get; set; }
        public User User { get; set; }
    }
}

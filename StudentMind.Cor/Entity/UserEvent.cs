
using StudentMind.Core.Base;

namespace StudentMind.Core.Entity
{
    public class UserEvent : BaseEntity
    {
        public string ProgramId { get; set; }
        public string UserId { get; set; }

        public Event Event { get; set; }
        public User User { get; set; }
    }
}

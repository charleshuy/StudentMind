
using StudentMind.Core.Base;

namespace StudentMind.Core.Entity
{
    public class History : BaseEntity
    {
        public DateTime Time { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }

        public User User { get; set; }
    }
}

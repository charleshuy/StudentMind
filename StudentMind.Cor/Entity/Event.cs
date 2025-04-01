using StudentMind.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace StudentMind.Core.Entity
{
    public class Event : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? HostId { get; set; }

        public User? Host { get; set; }
        [JsonIgnore]
        public ICollection<UserEvent>? UserEvents { get; set; }
    }
}

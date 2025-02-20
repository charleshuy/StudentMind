using StudentMind.Core.Base;


namespace StudentMind.Core.Entity
{
    public class Certificate : BaseEntity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public ICollection<User> Users { get; set; }
    }

}

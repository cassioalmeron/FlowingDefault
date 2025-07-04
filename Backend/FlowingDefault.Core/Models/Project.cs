namespace FlowingDefault.Core.Models
{
    public record Project : EntityBase
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
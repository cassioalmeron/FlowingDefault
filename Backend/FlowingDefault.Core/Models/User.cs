namespace FlowingDefault.Core.Models
{
    public record User : EntityBase
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
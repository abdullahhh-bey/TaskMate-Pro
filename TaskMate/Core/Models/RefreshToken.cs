namespace TaskMate.Core.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry {  get; set; }
        public bool IsChanged { get; set; }

        //Adding these as FOREIGN Key
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}

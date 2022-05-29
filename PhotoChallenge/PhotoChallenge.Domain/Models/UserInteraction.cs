using PhotoChallenge.Domain.Enums;

namespace PhotoChallenge.Domain.Models
{
    public class UserInteraction
    {
        public int Id { get; set; }
        public string? Photo { get; set; }
        public ApplicationUser User { get; set; }
        public Challenge Challenge { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Status { get; set; }
    }
}

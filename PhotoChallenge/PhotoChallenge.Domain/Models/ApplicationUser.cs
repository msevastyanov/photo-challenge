using Microsoft.AspNetCore.Identity;

namespace PhotoChallenge.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? DefaultAreaId { get; set; }
        public List<UserInteraction> UserInteractions { get; set; }
    }
}
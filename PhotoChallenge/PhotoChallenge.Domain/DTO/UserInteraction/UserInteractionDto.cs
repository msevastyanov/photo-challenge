using PhotoChallenge.Domain.Enums;

namespace PhotoChallenge.Domain.DTO.UserInteraction
{
    public class UserInteractionDto
    {
        public int Id { get; set; }
        public string? Photo { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string UserId { get; set; }
        public int ChallengeId { get; set; }
        public DateTime ChallengeStart { get; set; }
        public DateTime ChallengeEnd { get; set; }
        public string Description { get; set; }
        public int AreaId { get; set; }
        public string Area { get; set; }
        public string UserName { get; set; }
        public UserInteractionStatus Status { get; set; }
    }
}

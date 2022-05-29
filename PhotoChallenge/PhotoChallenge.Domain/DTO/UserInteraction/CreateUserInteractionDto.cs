using Microsoft.AspNetCore.Http;

namespace PhotoChallenge.Domain.DTO.UserInteraction
{
    public class CreateUserInteractionDto
    {
        public string FileName { get; set; }
        public int ChallengeId { get; set; }
    }
}

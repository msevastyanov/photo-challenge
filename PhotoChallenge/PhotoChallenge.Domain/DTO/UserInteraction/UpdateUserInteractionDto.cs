using PhotoChallenge.Domain.Enums;

namespace PhotoChallenge.Domain.DTO.UserInteraction
{
    public class UpdateUserInteractionDto
    {
        public int Id { get; set; }
        public UserInteractionStatus Status { get; set; }
    }
}

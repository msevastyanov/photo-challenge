namespace PhotoChallenge.Domain.DTO.User
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public int? DefaultAreaId { get; set; }
        public int Award { get; set; }
        public int Challenges { get; set; }
        public string? Role { get; set; }
    }
}

namespace PhotoChallenge.Domain.Models
{
    public class Area
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Challenge> Challenges { get; set; }
    }
}

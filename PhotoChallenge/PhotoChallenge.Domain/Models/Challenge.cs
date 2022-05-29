namespace PhotoChallenge.Domain.Models
{
    public class Challenge
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? Description { get; set; }
        public int Award { get; set; }
        public string Status { get; set; }
        public Area Area { get; set; }
    }
}

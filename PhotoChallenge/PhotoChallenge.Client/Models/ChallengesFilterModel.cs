namespace PhotoChallenge.Client.Models
{
    public class ChallengesFilterModel
    {
        public bool IsDraft { get; set; }
        public bool IsLive { get; set; }
        public bool IsActual { get; set; }
        public bool IsExpired { get; set; }
        public bool IsRemoved { get; set; }
        public int AreaId { get; set; }
    }
}

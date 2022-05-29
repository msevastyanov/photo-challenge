namespace PhotoChallenge.Client.Models
{
    public class UserInteractionFilterModel
    {
        public bool IsPending { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public int AreaId { get; set; }
    }
}

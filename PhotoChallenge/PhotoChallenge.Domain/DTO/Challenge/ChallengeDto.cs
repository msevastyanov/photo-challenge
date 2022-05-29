using PhotoChallenge.Domain.Enums;

namespace PhotoChallenge.Domain.DTO.Challenge
{
    public class ChallengeDto
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? Description { get; set; }
        public ChallengeAward Award { get; set; }
        public ChallengeStatus Status { get; set; }
        public int AreaId { get; set; }
        public string? AreaName { get; set; }
        public string CurrentStatus
        {
            get
            {
                if (Status != ChallengeStatus.Live)
                    return Status.ToString();

                if (DateTime.Now > DateEnd)
                    return "Expired";

                if (DateTime.Now.Date >= DateStart.Date && DateTime.Now.Date <= DateEnd.Date)
                    return "Actual";

                return Status.ToString();
            }
        }
    }
}
